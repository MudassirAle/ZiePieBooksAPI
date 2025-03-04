using Application.Interface;
using Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace Infrastructure.Service
{
    public class QBManualAccountService : IQBManualAccountService
    {
        private readonly string _connectionString;
        private readonly ILogger<QBManualAccountService> _logger;

        public QBManualAccountService(IConfiguration configuration, ILogger<QBManualAccountService> logger)
        {
            _connectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        public async Task<ServiceResponse<List<QBManualAccountResponse>>> GetByBusinessId(int businessId)
        {
            var response = new ServiceResponse<List<QBManualAccountResponse>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.QBManualAccount WHERE BusinessId = @BusinessId AND IsActive = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@BusinessId", businessId);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            List<QBManualAccountResponse> accounts = new List<QBManualAccountResponse>();

                            while (await dataReader.ReadAsync())
                            {
                                QBManualAccountResponse account = new QBManualAccountResponse
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    BusinessId = Convert.ToInt32(dataReader["BusinessId"]),
                                    OnboardingId = Convert.ToInt32(dataReader["OnboardingId"]),
                                    Name = dataReader["Title"].ToString() ?? string.Empty,
                                    AccountSubType = dataReader["Type"].ToString() ?? string.Empty,
                                    OpeningBalance = Convert.ToDecimal(dataReader["OpeningBalance"]),
                                    OpeningDate = Convert.ToDateTime(dataReader["OpeningDate"])
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

        public async Task<ServiceResponse<int?>> Post(QBManualAccount qbManualAccount)
        {
            var response = new ServiceResponse<int?>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "INSERT INTO zb.QBManualAccount (BusinessId, OnboardingId, Title, Type, OpeningBalance, OpeningDate) " +
                                 "VALUES (@BusinessId, @OnboardingId, @Title, @Type, @OpeningBalance, @OpeningDate); " +
                                 "SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@BusinessId", qbManualAccount.BusinessId);
                        command.Parameters.AddWithValue("@OnboardingId", qbManualAccount.OnboardingId);
                        command.Parameters.AddWithValue("@Title", qbManualAccount.Title);
                        command.Parameters.AddWithValue("@Type", qbManualAccount.Type);
                        command.Parameters.AddWithValue("@OpeningBalance", qbManualAccount.OpeningBalance);
                        command.Parameters.AddWithValue("@OpeningDate", qbManualAccount.OpeningDate);

                        int? lastInsertedId = Convert.ToInt32(await command.ExecuteScalarAsync());

                        if (lastInsertedId != null)
                        {
                            response.Data = lastInsertedId.Value;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Failed to insert QBManualAccount.";
                            _logger.LogError("Failed to insert QBManualAccount.");
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