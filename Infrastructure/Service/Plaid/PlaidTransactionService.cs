using Application.Interface;
using Core.Model;
using Core.Model.Plaid;
using Going.Plaid.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace Infrastructure.Service
{
    public class PlaidTransactionService : IPlaidTransactionService
    {
        private readonly string _connectionString;
        private readonly ILogger<PlaidTransactionService> _logger;

        public PlaidTransactionService(IConfiguration configuration, ILogger<PlaidTransactionService> logger)
        {
            _connectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        public async Task<ServiceResponse<PlaidTransaction>> GetByAccountId(string accountId)
        {
            var response = new ServiceResponse<PlaidTransaction>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.PlaidTransaction WHERE AccountId = @AccountId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AccountId", accountId);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                PlaidTransaction plaidTransaction = new PlaidTransaction
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    AccountId = dataReader["AccountId"].ToString() ?? string.Empty,
                                    TotalTransactions = Convert.ToInt32(dataReader["TotalTransactions"]),
                                    LastSync = Convert.ToDateTime(dataReader["LastSync"]),
                                    Transactions = JsonConvert.DeserializeObject<List<Transaction>>(dataReader["Transactions"].ToString() ?? string.Empty) ?? new List<Transaction>()
                                };
                                response.Data = plaidTransaction;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = "PlaidTransaction not found.";
                                _logger.LogError("PlaidTransaction not found.");
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                response.IsSuccess = false;
                _logger.LogError($"SQL Error: {sqlEx.Message}");
                response.ErrorMessage = $"SQL Error: {sqlEx.Message}";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                _logger.LogError($"An error occurred: {ex.Message}");
                response.ErrorMessage = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<int?>> Post(PlaidTransaction plaidTransaction)
        {
            var response = new ServiceResponse<int?>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "INSERT INTO zb.PlaidTransaction (AccountId, TotalTransactions, LastSync, Transactions) " +
                                 "VALUES (@AccountId, @TotalTransactions, @LastSync, @Transactions); " +
                                 "SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AccountId", plaidTransaction.AccountId);
                        command.Parameters.AddWithValue("@TotalTransactions", plaidTransaction.TotalTransactions);
                        command.Parameters.AddWithValue("@LastSync", plaidTransaction.LastSync);
                        command.Parameters.AddWithValue("@Transactions", JsonConvert.SerializeObject(plaidTransaction.Transactions));

                        int? lastInsertedId = Convert.ToInt32(await command.ExecuteScalarAsync());

                        if (lastInsertedId != null)
                        {
                            response.Data = lastInsertedId;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.ErrorMessage = "Failed to insert PlaidTransaction.";
                            _logger.LogError("Failed to insert PlaidTransaction.");
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                response.IsSuccess = false;
                _logger.LogError($"SQL Error: {sqlEx.Message}");
                response.ErrorMessage = $"SQL Error: {sqlEx.Message}";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                _logger.LogError($"An error occurred: {ex.Message}");
                response.ErrorMessage = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> Update(PlaidTransaction plaidTransaction)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE zb.PlaidTransaction SET " +
                                 "AccountId = @AccountId, " +
                                 "TotalTransactions = @TotalTransactions, " +
                                 "LastSync = @LastSync, " +
                                 "Transactions = @Transactions " +
                                 "WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", plaidTransaction.ID);
                        command.Parameters.AddWithValue("@AccountId", plaidTransaction.AccountId);
                        command.Parameters.AddWithValue("@TotalTransactions", plaidTransaction.TotalTransactions);
                        command.Parameters.AddWithValue("@LastSync", plaidTransaction.LastSync);
                        command.Parameters.AddWithValue("@Transactions", JsonConvert.SerializeObject(plaidTransaction.Transactions));

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        response.Data = rowsAffected > 0;
                        response.IsSuccess = rowsAffected > 0;

                        if (!response.IsSuccess)
                        {
                            response.ErrorMessage = "No rows affected.";
                            _logger.LogError("No rows affected.");
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                response.IsSuccess = false;
                _logger.LogError($"SQL Error: {sqlEx.Message}");
                response.ErrorMessage = $"SQL Error: {sqlEx.Message}";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                _logger.LogError($"An error occurred: {ex.Message}");
                response.ErrorMessage = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> Delete(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "DELETE FROM zb.PlaidTransaction WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        response.Data = rowsAffected > 0;
                        response.IsSuccess = rowsAffected > 0;

                        if (!response.IsSuccess)
                        {
                            response.ErrorMessage = "No rows affected.";
                            _logger.LogError("No rows affected.");
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                response.IsSuccess = false;
                _logger.LogError($"SQL Error: {sqlEx.Message}");
                response.ErrorMessage = $"SQL Error: {sqlEx.Message}";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                _logger.LogError($"An error occurred: {ex.Message}");
                response.ErrorMessage = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        /*Category = transaction.Category?.Select(c => new NormalizedCategory { Name = c }).ToList()*/

        //public static List<NormalizedTransaction> MapToNormalizedTransaction(List<Transaction> plaidTransactions)
        //{
        //    if (plaidTransactions == null || !plaidTransactions.Any())
        //    {
        //        return new List<NormalizedTransaction>();
        //    }

        //    return plaidTransactions.Select(transaction => new NormalizedTransaction
        //    {
        //        AccountId = transaction.AccountId,
        //        Amount = transaction.Amount,
        //        IsoCurrencyCode = transaction.IsoCurrencyCode,
        //        UnofficialCurrencyCode = transaction.UnofficialCurrencyCode,
        //        Category = transaction.Category?.Select(c => new NormalizedCategory { Name = c }).ToList(),
        //        CategoryId = transaction.CategoryId,
        //        CheckNumber = transaction.CheckNumber,
        //        Date = transaction.Date.HasValue ? DateOnly.FromDateTime(transaction.Date.Value) : null,
        //        Location = transaction.Location,
        //        Name = transaction.Name,
        //        MerchantName = transaction.MerchantName,
        //        OriginalDescription = transaction.OriginalDescription,
        //        PaymentMeta = transaction.PaymentMeta,
        //        Pending = transaction.Pending,
        //        PendingTransactionId = transaction.PendingTransactionId,
        //        AccountOwner = transaction.AccountOwner,
        //        TransactionId = transaction.TransactionId,
        //        TransactionType = transaction.TransactionType,
        //        LogoUrl = transaction.LogoUrl,
        //        Website = transaction.Website,
        //        AuthorizedDate = transaction.AuthorizedDate.HasValue ? DateOnly.FromDateTime(transaction.AuthorizedDate.Value) : null,
        //        AuthorizedDatetime = transaction.AuthorizedDatetime,
        //        Datetime = transaction.Datetime,
        //        PaymentChannel = transaction.PaymentChannel,
        //        PersonalFinanceCategory = transaction.PersonalFinanceCategory,
        //        TransactionCode = transaction.TransactionCode,
        //        PersonalFinanceCategoryIconUrl = transaction.PersonalFinanceCategoryIconUrl,
        //        Counterparties = transaction.Counterparties,
        //        MerchantEntityId = transaction.MerchantEntityId
        //    }).ToList();
        //}
    }
}