using Application.Interface.QBDesktop;
using Core.Model.QBDesktop;
using Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace Infrastructure.Service.QBDesktop
{
    public class QBTransactionService : IQBTransactionService
    {
        private readonly string _connectionString;
        private readonly ILogger<QBTransactionService> _logger;

        public QBTransactionService(IConfiguration configuration, ILogger<QBTransactionService> logger)
        {
            _connectionString = configuration.GetConnectionString("SAP")
                ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        public async Task<ServiceResponse<List<QBTransaction>>> GetByTicket(string ticket, string startDate, string endDate)
        {
            var response = new ServiceResponse<List<QBTransaction>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = @"
                        SELECT AccountRef_FullName AS AccountTitle, TxnDate AS Date, Memo AS Description, 'Check' AS Type, 
                               ExpenseLineRet_ClassRef_FullName AS Class, RefNumber AS CheckNo, Amount
                        FROM dbo.WCCheck
                        WHERE Ticket = @Ticket 
                        AND TxnDate BETWEEN @startDate AND @endDate

                        UNION ALL

                        SELECT DepositToAccountRef_FullName AS AccountTitle, TxnDate AS Date, Memo AS Description, 'Deposit' AS Type, 
                               NULL AS Class, NULL AS CheckNo, DepositTotal AS Amount
                        FROM dbo.WCDeposit
                        WHERE Ticket = @Ticket 
                        AND TxnDate BETWEEN @startDate AND @endDate

                        UNION ALL

                        SELECT DepositToAccountRef_FullName AS AccountTitle, TxnDate AS Date, Memo AS Description, 'SalesReceipt' AS Type, 
                               ClassRef_FullName AS Class, CheckNumber AS CheckNo, TotalAmount AS Amount
                        FROM dbo.WCSalesReceipt
                        WHERE Ticket = @Ticket 
                        AND TxnDate BETWEEN @startDate AND @endDate

                        UNION ALL

                        SELECT TransferToAccountRef_FullName AS AccountTitle, TxnDate AS Date, Memo AS Description, 'Transfer' AS Type, 
                               ClassRef_FullName AS Class, NULL AS CheckNo, Amount
                        FROM dbo.WCTransfer
                        WHERE Ticket = @Ticket 
                        AND TxnDate BETWEEN @startDate AND @endDate";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Ticket", ticket);
                        command.Parameters.AddWithValue("@startDate", startDate);
                        command.Parameters.AddWithValue("@endDate", endDate);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            List<QBTransaction> transactions = new List<QBTransaction>();

                            while (await dataReader.ReadAsync())
                            {
                                QBTransaction transaction = new QBTransaction
                                {
                                    AccountTitle = dataReader["AccountTitle"]?.ToString() ?? string.Empty,
                                    Date = dataReader["Date"] != DBNull.Value ? Convert.ToDateTime(dataReader["Date"]) : DateTime.MinValue,
                                    Description = dataReader["Description"]?.ToString(),
                                    Type = dataReader["Type"]?.ToString() ?? string.Empty,
                                    Class = dataReader["Class"]?.ToString(),
                                    CheckNo = dataReader["CheckNo"]?.ToString(),
                                    Amount = dataReader["Amount"] != DBNull.Value ? Convert.ToDecimal(dataReader["Amount"]) : 0
                                };

                                transactions.Add(transaction);
                            }

                            response.Data = transactions;
                            response.IsSuccess = true;
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
    }
}