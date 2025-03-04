using Application.Interface;
using Core.Model.Plaid;
using Going.Plaid.Transactions;
using Going.Plaid;
using Microsoft.Extensions.Logging;
using Environment = Going.Plaid.Environment;
using Going.Plaid.Entity;
using System.Linq;
using Newtonsoft.Json;

namespace Infrastructure.Service
{
    public class PlaidSyncService : IPlaidSyncService
    {
        private readonly IPlaidAccountService _plaidAccountService;
        private readonly IPlaidTransactionService _plaidTransactionService;
        private readonly ILogger<PlaidSyncService> _logger;

        public PlaidSyncService(
            IPlaidAccountService plaidAccountService,
            IPlaidTransactionService plaidTransactionService,
            ILogger<PlaidSyncService> logger)
        {
            _plaidAccountService = plaidAccountService ?? throw new ArgumentNullException(nameof(plaidAccountService));
            _plaidTransactionService = plaidTransactionService ?? throw new ArgumentNullException(nameof(plaidTransactionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RunDailySync()
        {
            try
            {
                var plaidAccountsResponse = await _plaidAccountService.GetAll();
                if (!plaidAccountsResponse.IsSuccess || plaidAccountsResponse.Data == null)
                {
                    _logger.LogError("Failed to fetch Plaid accounts.");
                    return;
                }

                //var readyPlaidAccounts = plaidAccountsResponse.Data
                //    .Where(account => account.Status == "Ready")
                //    .ToList();

                var readyPlaidAccounts = plaidAccountsResponse.Data
                    .Where(account => account.Status == "Ready" && account.AccountId == "JKr5p8omqbhqV73pZrLpSLKY9ox3b7F3oZzb54")
                    .ToList();

                //Define a list of AccountIds to filter by
                //var accountIds = new List<string>
                //{
                //    "0Jx1qYvMk7sP8Ly68J1ds01KOdamKeHv1RXrO",
                //    "QLXV7jnMmQfjYAZXLX7ECL5AD9B4bMf4XDNZZ",
                //    "RDnEKrpM94HrYv5wDwENiyAXbDEq35iqNQOvA",
                //    "XLM9mvvOJruMkqqwE5Z8h7ZVowYjaDs6krD63",
                //    "5KgAnVAqXnTjbwNngXrJUO5vnZZp1PIVw18g9",
                //    "B789wwjZ5QSEx3MNovbJiDB6JvxkRwf45DwJP",
                //};

                //// Filter accounts using LINQ
                //var readyPlaidAccounts = plaidAccountsResponse.Data
                //    .Where(account => account.Status == "Ready" && accountIds.Contains(account.AccountId))
                //    .ToList();


                foreach (var plaidAccount in readyPlaidAccounts)
                {
                    var plaidTransactionResponse = await _plaidTransactionService.GetByAccountId(plaidAccount.AccountId);

                    //plaidTransactionResponse.IsSuccess = false;
                    //plaidTransactionResponse.Data = null;

                    if (!plaidTransactionResponse.IsSuccess || plaidTransactionResponse.Data == null)
                    {
                        _logger.LogInformation($"Fetching transactions for new account: {plaidAccount.AccountId}");

                        var fetchedTransactions = await FetchTransactionsFromPlaid(plaidAccount);

                        if (fetchedTransactions != null && fetchedTransactions.Any())
                        {
                            var plaidTransaction = new PlaidTransaction
                            {
                                AccountId = plaidAccount.AccountId,
                                Transactions = fetchedTransactions,
                                TotalTransactions = fetchedTransactions.Count,
                                LastSync = plaidAccount.LinkedAt
                            };
                            
                            var dbResponse = await _plaidTransactionService.Post(plaidTransaction);
                            if (!dbResponse.IsSuccess)
                            {
                                _logger.LogError($"Failed to post transactions for account {plaidAccount.AccountId}.");
                            }
                            else
                            {
                                _logger.LogInformation($"Successfully fetched and posted transactions for account: {plaidAccount.AccountId}");
                            }
                        }
                        else
                        {
                            _logger.LogWarning($"No transactions found for account {plaidAccount.AccountId}.");
                        }
                    }
                    else
                    {
                        var lastSync = plaidTransactionResponse.Data.LastSync;
                        var totalDays = DateTime.UtcNow.Subtract(lastSync).TotalDays;

                        if (totalDays >= 3)
                        {
                            _logger.LogInformation($"Syncing transactions for account: {plaidAccount.AccountId}");
                            var fetchedTransactions = await SyncTransactionsFromPlaid(plaidAccount, lastSync);

                            if (fetchedTransactions != null && fetchedTransactions.Any())
                            {
                                // Update transactions in the database
                                //plaidTransactionResponse.Data.Transactions = plaidTransactionResponse.Data.Transactions
                                //    .Concat(fetchedTransactions.Where(nt => !plaidTransactionResponse.Data.Transactions.Any(et => et.TransactionId == nt.TransactionId)))
                                //    .ToList();

                                var existingTransactions = plaidTransactionResponse.Data.Transactions;
                                var duplicateTransactions = fetchedTransactions.Where(nt => existingTransactions.Any(et => et.TransactionId == nt.TransactionId)).ToList();
                                var newTransactions = fetchedTransactions.Where(nt => !existingTransactions.Any(et => et.TransactionId == nt.TransactionId)).ToList();

                                plaidTransactionResponse.Data.Transactions = existingTransactions.Concat(newTransactions).ToList();

                                plaidTransactionResponse.Data.TotalTransactions = plaidTransactionResponse.Data.Transactions.Count;
                                plaidTransactionResponse.Data.LastSync = DateTime.UtcNow;

                                var dbResponse = await _plaidTransactionService.Update(plaidTransactionResponse.Data);
                                if (!dbResponse.IsSuccess)
                                {
                                    _logger.LogError($"Failed to update transactions for account {plaidAccount.AccountId}.");
                                }
                                else
                                {
                                    _logger.LogInformation($"Successfully synced transactions for account: {plaidAccount.AccountId}");
                                }
                            }
                            else
                            {
                                _logger.LogWarning($"No transactions found for account {plaidAccount.AccountId}.");
                            }
                        }
                        else
                        {
                            _logger.LogInformation($"No need to sync transactions for account: {plaidAccount.AccountId}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while syncing Plaid accounts: {ex.Message}");
            }
        }

        private async Task<List<Transaction>> FetchTransactionsFromPlaid(PlaidAccount account)
        {
            var client = new PlaidClient(
                Environment.Production,
                clientId: "65258da5ce8178001ccad5b7",
                secret: "0066bc5606992eefb2eb96163d2f6e"
            );

            DateTime startDate = account.LinkedAt.Subtract(TimeSpan.FromDays(730));
            DateTime endDate = account.LinkedAt;

            var transactions = new List<Transaction>();
            int offset = 0;
            int count = 500;

            try
            {
                while (true)
                {
                    var request = new TransactionsGetRequest
                    {
                        AccessToken = account.AccessToken,
                        StartDate = DateOnly.FromDateTime(startDate),
                        EndDate = DateOnly.FromDateTime(endDate),
                        Options = new TransactionsGetRequestOptions
                        {
                            AccountIds = new List<string> { account.AccountId },
                            Count = count,
                            Offset = offset
                        }
                    };

                    var result = await client.TransactionsGetAsync(request);

                    if (result.IsSuccessStatusCode)
                    {
                        transactions.AddRange(result.Transactions);
                        if (transactions.Count >= result.TotalTransactions)
                        {
                            break;
                        }

                        offset += count;
                    }
                    else
                    {
                        _logger.LogError($"Error fetching transactions for AccountId {account.AccountId}: {result.Error?.ErrorMessage}");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching transactions for AccountId {account.AccountId}: {ex.Message}");
            }

            return transactions;
        }

        private async Task<List<Transaction>> SyncTransactionsFromPlaid(PlaidAccount account, DateTime lastSync)
        {
            var client = new PlaidClient(
                Environment.Production,
                clientId: "65258da5ce8178001ccad5b7",
                secret: "0066bc5606992eefb2eb96163d2f6e"
            );

            DateTime startDate = lastSync;
            DateTime endDate = DateTime.UtcNow;

            var transactions = new List<Transaction>();
            int offset = 0;
            int count = 500;

            try
            {
                while (true)
                {
                    var request = new TransactionsGetRequest
                    {
                        AccessToken = account.AccessToken,
                        StartDate = DateOnly.FromDateTime(startDate),
                        EndDate = DateOnly.FromDateTime(endDate),
                        Options = new TransactionsGetRequestOptions
                        {
                            AccountIds = new List<string> { account.AccountId },
                            Count = count,
                            Offset = offset
                        }
                    };

                    var result = await client.TransactionsGetAsync(request);

                    if (result.IsSuccessStatusCode)
                    {
                        transactions.AddRange(result.Transactions);
                        if (transactions.Count >= result.TotalTransactions)
                        {
                            break;
                        }

                        offset += count;
                    }
                    else
                    {
                        _logger.LogError($"Error fetching transactions for AccountId {account.AccountId}: {result.Error?.DisplayMessage}");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching transactions for AccountId {account.AccountId}: {ex.Message}");
            }

            return transactions;
        }
    }
}