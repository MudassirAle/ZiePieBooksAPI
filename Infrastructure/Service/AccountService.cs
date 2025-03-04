using Application.Interface;
using Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace Infrastructure.Service
{
    public class AccountService : IAccountService
    {
        private readonly string _connectionString;
        private readonly ILogger<AccountService> _logger;

        public AccountService(IConfiguration configuration, ILogger<AccountService> logger)
        {
            _connectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        public async Task<ServiceResponse<List<Account>>> GetAll()
        {
            var response = new ServiceResponse<List<Account>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.Account WHERE IsActive = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                    {
                        List<Account> accounts = new List<Account>();

                        while (await dataReader.ReadAsync())
                        {
                            Account account = new Account
                            {
                                ID = Convert.ToInt32(dataReader["ID"]),
                                BusinessId = Convert.ToInt32(dataReader["BusinessId"]),
                                Title = dataReader["Title"].ToString() ?? string.Empty,
                                Type = dataReader["Type"].ToString() ?? string.Empty,
                                AccountNumber = dataReader["AccountNumber"].ToString() ?? string.Empty
                            };
                            accounts.Add(account);
                        }

                        response.Data = accounts;
                        response.IsSuccess = true;
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

        public async Task<ServiceResponse<List<Account>>> GetByBusinessId(int businessId)
        {
            var response = new ServiceResponse<List<Account>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.Account WHERE BusinessId = @BusinessId AND IsActive = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@BusinessId", businessId);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            List<Account> accounts = new List<Account>();

                            while (await dataReader.ReadAsync())
                            {
                                Account account = new Account
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    BusinessId = Convert.ToInt32(dataReader["BusinessId"]),
                                    Title = dataReader["Title"].ToString() ?? string.Empty,
                                    Type = dataReader["Type"].ToString() ?? string.Empty,
                                    AccountNumber = dataReader["AccountNumber"].ToString() ?? string.Empty
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

        public async Task<ServiceResponse<Account>> GetById(int id)
        {
            var response = new ServiceResponse<Account>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.Account WHERE ID = @ID AND IsActive = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                Account account = new Account
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    BusinessId = Convert.ToInt32(dataReader["BusinessId"]),
                                    Title = dataReader["Title"].ToString() ?? string.Empty,
                                    Type = dataReader["Type"].ToString() ?? string.Empty,
                                    AccountNumber = dataReader["AccountNumber"].ToString() ?? string.Empty
                                };
                                response.Data = account;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = "Account not found.";
                                _logger.LogError("Account not found.");
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

        public async Task<ServiceResponse<int?>> Post(Account account)
        {
            var response = new ServiceResponse<int?>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "INSERT INTO zb.Account (BusinessId, Title, Type, AccountNumber) " +
                                 "VALUES (@BusinessId, @Title, @Type, @AccountNumber); " +
                                 "SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@BusinessId", account.BusinessId);
                        command.Parameters.AddWithValue("@Title", account.Title);
                        command.Parameters.AddWithValue("@Type", account.Type);
                        command.Parameters.AddWithValue("@AccountNumber", account.AccountNumber);

                        int? lastInsertedId = Convert.ToInt32(await command.ExecuteScalarAsync());

                        if (lastInsertedId != null)
                        {
                            response.Data = lastInsertedId.Value;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Failed to insert account.";
                            _logger.LogError("Failed to insert account.");
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

        public async Task<ServiceResponse<bool>> Update(Account account)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE zb.Account SET BusinessId = @BusinessId, " +
                                 "Title = @Title, Type = @Type, AccountNumber = @AccountNumber " +
                                 "WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@BusinessId", account.BusinessId);
                        command.Parameters.AddWithValue("@Title", account.Title);
                        command.Parameters.AddWithValue("@Type", account.Type);
                        command.Parameters.AddWithValue("@AccountNumber", account.AccountNumber);
                        command.Parameters.AddWithValue("@ID", account.ID);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        response.IsSuccess = rowsAffected > 0;
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

                    string sql = "Update zb.Account SET IsActive = 0 WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        response.IsSuccess = rowsAffected > 0;
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