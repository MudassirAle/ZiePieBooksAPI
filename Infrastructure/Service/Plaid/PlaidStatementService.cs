using Application.Interface;
using Core.Model;
using Core.Model.Plaid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace Infrastructure.Service
{
    public class PlaidStatementService : IPlaidStatementService
    {
        private readonly string _connectionString;
        private readonly ILogger<PlaidStatementService> _logger;

        public PlaidStatementService(IConfiguration configuration, ILogger<PlaidStatementService> logger)
        {
            _connectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        public async Task<ServiceResponse<PlaidStatement>> GetByStatementId(string statementId)
        {
            var response = new ServiceResponse<PlaidStatement>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.PlaidStatement WHERE StatementId = @StatementId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@StatementId", statementId);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                PlaidStatement plaidStatement = new PlaidStatement
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    AccountId = Convert.ToInt32(dataReader["AccountId"]),
                                    StatementId = dataReader["StatementId"].ToString() ?? string.Empty,
                                    Year = dataReader["Year"].ToString() ?? string.Empty,
                                    Month = dataReader["Month"].ToString() ?? string.Empty,
                                    BlobUri = dataReader["BlobUri"].ToString() ?? string.Empty
                                };
                                response.Data = plaidStatement;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = "PlaidStatement not found.";
                                _logger.LogError("PlaidStatement not found.");
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

        public async Task<ServiceResponse<List<PlaidStatement>>> GetByAccountId(int accountId)
        {
            var response = new ServiceResponse<List<PlaidStatement>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.PlaidStatement WHERE AccountId = @AccountId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AccountId", accountId);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            var plaidStatements = new List<PlaidStatement>();

                            while (await dataReader.ReadAsync())
                            {
                                PlaidStatement plaidStatement = new PlaidStatement
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    AccountId = Convert.ToInt32(dataReader["AccountId"]),
                                    StatementId = dataReader["StatementId"].ToString() ?? string.Empty,
                                    Year = dataReader["Year"].ToString() ?? string.Empty,
                                    Month = dataReader["Month"].ToString() ?? string.Empty,
                                    BlobUri = dataReader["BlobUri"].ToString() ?? string.Empty
                                };

                                plaidStatements.Add(plaidStatement);
                            }

                            if (plaidStatements.Count > 0)
                            {
                                response.Data = plaidStatements;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = "No PlaidStatements found.";
                                _logger.LogError("No PlaidStatements found.");
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


        public async Task<ServiceResponse<int?>> Post(PlaidStatement plaidStatement)
        {
            var response = new ServiceResponse<int?>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "INSERT INTO zb.PlaidStatement (AccountId, StatementId, Year, Month, BlobUri) " +
                                 "VALUES (@AccountId, @StatementId, @Year, @Month, @BlobUri); " +
                                 "SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AccountId", plaidStatement.AccountId);
                        command.Parameters.AddWithValue("@StatementId", plaidStatement.StatementId);
                        command.Parameters.AddWithValue("@Year", plaidStatement.Year);
                        command.Parameters.AddWithValue("@Month", plaidStatement.Month);
                        command.Parameters.AddWithValue("@BlobUri", plaidStatement.BlobUri);

                        int? lastInsertedId = Convert.ToInt32(await command.ExecuteScalarAsync());

                        if (lastInsertedId != null)
                        {
                            response.Data = lastInsertedId;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.ErrorMessage = "Failed to insert PlaidStatement.";
                            _logger.LogError("Failed to insert PlaidStatement.");
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

        public async Task<ServiceResponse<bool>> Update(PlaidStatement plaidStatement)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE zb.PlaidStatement SET " +
                                 "AccountId = @AccountId, " +
                                 "StatementId = @StatementId, " +
                                 "Year = @Year, " +
                                 "Month = @Month, " +
                                 "BlobUri = @BlobUri " +
                                 "WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", plaidStatement.ID);
                        command.Parameters.AddWithValue("@AccountId", plaidStatement.AccountId);
                        command.Parameters.AddWithValue("@StatementId", plaidStatement.StatementId);
                        command.Parameters.AddWithValue("@Year", plaidStatement.Year);
                        command.Parameters.AddWithValue("@Month", plaidStatement.Month);
                        command.Parameters.AddWithValue("@BlobUri", plaidStatement.BlobUri);

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

                    string sql = "DELETE FROM zb.PlaidStatement WHERE ID = @ID";

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
    }
}