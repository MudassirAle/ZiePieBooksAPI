using Application.Interface.QBDesktop;
using Core.Model.QBDesktop;
using Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace Infrastructure.Service.QBDesktop
{
    public class QBTransactionReportService : IQBTransactionReportService
    {
        private readonly string _connectionString;
        private readonly ILogger<QBTransactionReportService> _logger;

        public QBTransactionReportService(IConfiguration configuration, ILogger<QBTransactionReportService> logger)
        {
            _connectionString = configuration.GetConnectionString("SAP")
                ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        public async Task<ServiceResponse<List<QBTransactionReport>>> GetByTicket(string ticket, string startDate, string endDate)
        {
            var response = new ServiceResponse<List<QBTransactionReport>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = @"
                        SELECT Account AS AccountTitle, Date AS Date, Memo AS Description, TxnType AS Type, 
                               Class AS Class, RefNumber AS CheckNo, Amount AS Amount 
                        FROM dbo.WCTransactionReport 
                        WHERE Ticket = @Ticket 
                        AND Date BETWEEN @startDate AND @endDate";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Ticket", ticket);
                        command.Parameters.AddWithValue("@startDate", startDate);
                        command.Parameters.AddWithValue("@endDate", endDate);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            List<QBTransactionReport> reports = new List<QBTransactionReport>();

                            while (await dataReader.ReadAsync())
                            {
                                QBTransactionReport report = new QBTransactionReport
                                {
                                    AccountTitle = dataReader["AccountTitle"]?.ToString() ?? string.Empty,
                                    Date = dataReader["Date"] != DBNull.Value ? Convert.ToDateTime(dataReader["Date"]) : DateTime.MinValue,
                                    Description = dataReader["Description"]?.ToString(),
                                    Type = dataReader["Type"]?.ToString() ?? string.Empty,
                                    Class = dataReader["Class"]?.ToString(),
                                    CheckNo = dataReader["CheckNo"]?.ToString(),
                                    Amount = dataReader["Amount"] != DBNull.Value ? Convert.ToDecimal(dataReader["Amount"]) : 0
                                };

                                reports.Add(report);
                            }

                            response.Data = reports;
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