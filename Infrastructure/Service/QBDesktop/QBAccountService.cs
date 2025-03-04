using Application.Interface.QBDesktop;
using Core.Model;
using Core.Model.QBDesktop;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace Infrastructure.Service.QBDesktop
{
    public class QBAccountService : IQBAccountService
    {
        private readonly string _connectionString;
        private readonly ILogger<QBAccountService> _logger;

        public QBAccountService(IConfiguration configuration, ILogger<QBAccountService> logger)
        {
            _connectionString = configuration.GetConnectionString("SAP")
                ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        public async Task<ServiceResponse<List<QBAccount>>> GetByTicket(string ticket)
        {
            var response = new ServiceResponse<List<QBAccount>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM dbo.WCAccount WHERE Ticket = @Ticket";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Ticket", ticket);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            List<QBAccount> accounts = new List<QBAccount>();

                            while (await dataReader.ReadAsync())
                            {
                                QBAccount account = new QBAccount
                                {
                                    Id = Convert.ToInt32(dataReader["ID"]),
                                    Ticket = dataReader["Ticket"]?.ToString() ?? string.Empty,
                                    ListID = dataReader["ListID"]?.ToString() ?? string.Empty,
                                    TimeCreated = dataReader["TimeCreated"] != DBNull.Value
                                        ? Convert.ToDateTime(dataReader["TimeCreated"])
                                        : default,
                                    Name = dataReader["Name"]?.ToString() ?? string.Empty,
                                    FullName = dataReader["FullName"]?.ToString() ?? string.Empty,
                                    IsActive = dataReader["IsActive"]?.ToString() ?? string.Empty,
                                    AccountType = dataReader["AccountType"]?.ToString() ?? string.Empty,
                                    Balance = dataReader["Balance"] != DBNull.Value
                                        ? Convert.ToDecimal(dataReader["Balance"])
                                        : 0,
                                    TotalBalance = dataReader["TotalBalance"] != DBNull.Value
                                        ? Convert.ToDecimal(dataReader["TotalBalance"])
                                        : 0
                                };
                                accounts.Add(account);
                            }

                            response.Data = accounts;
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