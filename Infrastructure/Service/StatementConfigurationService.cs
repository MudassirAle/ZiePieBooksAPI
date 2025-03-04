using Application.Interface;
using Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
    public class StatementConfigurationService : IStatementConfigurationService
    {
        private readonly string _connectionString;
        private readonly ILogger<StatementConfigurationService> _logger;

        public StatementConfigurationService(
            IConfiguration configuration,
            ILogger<StatementConfigurationService> logger)
        {
            _connectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        public async Task<ServiceResponse<StatementConfiguration>> GetByPlatformAccountId(int platformAccountId)
        {
            var response = new ServiceResponse<StatementConfiguration>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.StatementConfiguration WHERE PlatformAccountId = @PlatformAccountId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@PlatformAccountId", platformAccountId);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                StatementConfiguration statementConfiguration = new StatementConfiguration
                                {
                                    ID = dataReader["ID"] != DBNull.Value ? Convert.ToInt32(dataReader["ID"]) : 0,
                                    BusinessId = dataReader["BusinessId"] != DBNull.Value ? Convert.ToInt32(dataReader["BusinessId"]) : 0,
                                    OnboardingId = dataReader["OnboardingId"] != DBNull.Value ? Convert.ToInt32(dataReader["OnboardingId"]) : 0,
                                    PlatformAccountId = dataReader["PlatformAccountId"] != DBNull.Value ? Convert.ToInt32(dataReader["PlatformAccountId"]) : 0,
                                    IsFirstRowHeader = dataReader["IsFirstRowHeader"] != DBNull.Value ? Convert.ToBoolean(dataReader["IsFirstRowHeader"]) : false,
                                    NoOfAmountColumns = dataReader["NoOfAmountColumns"] != DBNull.Value ? Convert.ToInt32(dataReader["NoOfAmountColumns"]) : 0,
                                    DateFormat = dataReader["DateFormat"] != DBNull.Value ? dataReader["DateFormat"].ToString()! : string.Empty,
                                    //Columns = JsonConvert.DeserializeObject<List<ColumnConfiguration>>(dataReader["Columns"].ToString() ?? string.Empty) ?? new List<ColumnConfiguration>()
                                    Columns = dataReader["Columns"] != DBNull.Value ? JsonConvert.DeserializeObject<List<ColumnConfiguration>>(dataReader["Columns"].ToString() ?? string.Empty) ?? new List<ColumnConfiguration>() : new List<ColumnConfiguration>()
                                };

                                response.Data = statementConfiguration;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = "StatementConfiguration record not found.";
                                _logger.LogError("StatementConfiguration record not found for PlatformAccountId: {PlatformAccountId}", platformAccountId);
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

        public async Task<ServiceResponse<int?>> PostStatementConfiguration(StatementConfigurationDTO statementConfigurationDTO)
        {
            var response = new ServiceResponse<int?>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sqlInsert = @"
                        INSERT INTO zb.StatementConfiguration 
                        (BusinessId, OnboardingId, PlatformAccountId, IsFirstRowHeader, NoOfAmountColumns, DateFormat)
                        VALUES 
                        (@BusinessId, @OnboardingId, @PlatformAccountId, @IsFirstRowHeader, @NoOfAmountColumns, @DateFormat);
                        SELECT SCOPE_IDENTITY();";

                    using (SqlCommand insertCommand = new SqlCommand(sqlInsert, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@BusinessId", statementConfigurationDTO.BusinessId);
                        insertCommand.Parameters.AddWithValue("@OnboardingId", statementConfigurationDTO.OnboardingId);
                        insertCommand.Parameters.AddWithValue("@PlatformAccountId", statementConfigurationDTO.PlatformAccountId);
                        insertCommand.Parameters.AddWithValue("@IsFirstRowHeader", statementConfigurationDTO.IsFirstRowHeader);
                        insertCommand.Parameters.AddWithValue("@NoOfAmountColumns", statementConfigurationDTO.NoOfAmountColumns);
                        insertCommand.Parameters.AddWithValue("@DateFormat", statementConfigurationDTO.DateFormat);

                        int? lastInsertedId = Convert.ToInt32(await insertCommand.ExecuteScalarAsync());

                        if (lastInsertedId != null)
                        {
                            response.Data = lastInsertedId.Value;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Failed to insert statement configuration.";
                            _logger.LogError("Failed to insert statement configuration.");
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

        public async Task<ServiceResponse<int?>> PostColumnConfiguration(ColumnConfigurationDTO columnConfigurationDTO)
        {
            var response = new ServiceResponse<int?>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string columnsJson = JsonConvert.SerializeObject(columnConfigurationDTO.Columns);

                    string sqlUpdateColumns = @"
                        UPDATE zb.StatementConfiguration
                        SET Columns = @Columns
                        WHERE ID = @StatementConfigurationId;";

                    using (SqlCommand updateCommand = new SqlCommand(sqlUpdateColumns, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@StatementConfigurationId", columnConfigurationDTO.StatementConfigurationId);
                        updateCommand.Parameters.AddWithValue("@Columns", columnsJson);

                        int rowsAffected = await updateCommand.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            response.IsSuccess = true;
                            response.Data = columnConfigurationDTO.StatementConfigurationId;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "No records were updated. Please check if the StatementConfigurationId is valid.";
                            _logger.LogWarning($"No records were updated for StatementConfigurationId: {columnConfigurationDTO.StatementConfigurationId}");
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