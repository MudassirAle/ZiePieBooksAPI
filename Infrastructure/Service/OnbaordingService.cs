using Application.Interface;
using Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace Infrastructure.Service
{
    public class OnboardingService : IOnboardingService
    {
        private readonly string _connectionString;
        private readonly ILogger<OnboardingService> _logger;

        public OnboardingService(
            IConfiguration configuration,
            ILogger<OnboardingService> logger
        )
        {
            _connectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        public async Task<ServiceResponse<Onboarding>> GetByBusinessID(int businessId)
        {
            var response = new ServiceResponse<Onboarding>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.Onboarding WHERE BusinessId = @BusinessId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@BusinessId", businessId);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                Onboarding onboarding = new Onboarding
                                {
                                    ID = dataReader["ID"] != DBNull.Value ? Convert.ToInt32(dataReader["ID"]) : 0,
                                    BusinessId = dataReader["BusinessId"] != DBNull.Value ? Convert.ToInt32(dataReader["BusinessId"]) : 0,
                                    StepNumber = dataReader["StepNumber"] != DBNull.Value ? Convert.ToInt32(dataReader["StepNumber"]) : 0,
                                    StepName = dataReader["StepName"] != DBNull.Value ? dataReader["StepName"].ToString()! : string.Empty,
                                    DataSource = dataReader["DataSource"] != DBNull.Value ? JsonConvert.DeserializeObject<DataSource>(dataReader["DataSource"].ToString()!) : null
                                };

                                response.Data = onboarding;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = "Onboarding record not found.";
                                _logger.LogError("Onboarding record not found.");
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

        public async Task<ServiceResponse<int?>> Post(OnboardingDTO onboardingDTO)
        {
            var response = new ServiceResponse<int?>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "INSERT INTO zb.Onboarding (BusinessId, StepNumber, StepName) " +
                                 "VALUES (@BusinessId, @StepNumber, @StepName); " +
                                 "SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@BusinessId", onboardingDTO.BusinessId);
                        command.Parameters.AddWithValue("@StepNumber", onboardingDTO.StepNumber);
                        command.Parameters.AddWithValue("@StepName", onboardingDTO.StepName);

                        int? lastInsertedId = Convert.ToInt32(await command.ExecuteScalarAsync());

                        if (lastInsertedId != null)
                        {
                            response.Data = lastInsertedId.Value;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Failed to insert onboarding.";
                            _logger.LogError("Failed to insert onboarding.");
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

        public async Task<ServiceResponse<bool>> UpdateDataSource(DataSourceDTO dataSourceDTO)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string dataSourceJson = JsonConvert.SerializeObject(dataSourceDTO.DataSource);

                    string sql = "UPDATE zb.Onboarding " +
                                 "SET StepNumber = @StepNumber, StepName = @StepName, DataSource = @DataSource " +
                                 "WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@StepNumber", dataSourceDTO.StepNumber);
                        command.Parameters.AddWithValue("@StepName", dataSourceDTO.StepName);
                        command.Parameters.AddWithValue("@DataSource", dataSourceJson);
                        command.Parameters.AddWithValue("@ID", dataSourceDTO.ID);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            response.Data = true;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "No records were updated. Record may not exist.";
                            _logger.LogError($"No record found to update with ID {dataSourceDTO.ID}.");
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

        //public async Task<ServiceResponse<List<OnboardingForAdmin>>> GetForAdmin()
        //{
        //    var response = new ServiceResponse<List<OnboardingForAdmin>>();

        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(_connectionString))
        //        {
        //            await connection.OpenAsync();

        //            string sql = @"
        //                SELECT 
        //                    t.Name AS Tenant, 
        //                    c.Name AS Customer, 
        //                    b.Name AS Business,
        //                    o.Platform,
        //                    q.ReceivedAt,
        //                    q.Status
        //                FROM 
        //                    zb.Onboarding o
        //                INNER JOIN 
        //                    zb.Business b ON o.BusinessId = b.ID
        //                INNER JOIN 
        //                    zb.Customer c ON b.CustomerId = c.ID
        //                INNER JOIN 
        //                    zb.Tenant t ON c.TenantId = t.ID
        //                LEFT JOIN 
        //                    zb.QBFileSync q ON o.BusinessId = q.BusinessId
        //                WHERE 
        //                    o.Platform = 'QBDesktop';
        //            ";

        //            using (SqlCommand command = new SqlCommand(sql, connection))
        //            using (SqlDataReader reader = await command.ExecuteReaderAsync())
        //            {
        //                var onboardingForAdmins = new List<OnboardingForAdmin>();

        //                while (await reader.ReadAsync())
        //                {
        //                    var hierarchy = new Hierarchy
        //                    {
        //                        Tenant = reader["Tenant"].ToString()!,
        //                        Customer = reader["Customer"].ToString()!,
        //                        Business = reader["Business"].ToString()!
        //                    };

        //                    var onboardingForAdmin = new OnboardingForAdmin
        //                    {
        //                        Hierarchy = hierarchy,
        //                        Platform = reader["Platform"].ToString()!,
        //                        ReceivedAt = reader["ReceivedAt"] != DBNull.Value
        //                            ? Convert.ToDateTime(reader["ReceivedAt"])
        //                            : default(DateTime),
        //                        Status = reader["Status"]?.ToString() ?? string.Empty
        //                    };

        //                    onboardingForAdmins.Add(onboardingForAdmin);
        //                }

        //                response.Data = onboardingForAdmins;
        //                response.IsSuccess = true;
        //            }
        //        }
        //    }
        //    catch (SqlException sqlEx)
        //    {
        //        response.IsSuccess = false;
        //        response.ErrorMessage = $"SQL Error: {sqlEx.Message}";
        //        _logger.LogError($"SQL Error: {sqlEx.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        response.ErrorMessage = $"An error occurred: {ex.Message}";
        //        _logger.LogError($"An error occurred: {ex.Message}");
        //    }

        //    return response;
        //}

        public async Task<ServiceResponse<List<OnboardingForAdmin>>> GetForAdmin()
        {
            var response = new ServiceResponse<List<OnboardingForAdmin>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("OnboardingForAdmin", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            var onboardingForAdmins = new List<OnboardingForAdmin>();

                            while (await reader.ReadAsync())
                            {
                                var hierarchy = new Hierarchy
                                {
                                    Tenant = reader["Tenant"].ToString() ?? string.Empty,
                                    Customer = reader["Customer"].ToString() ?? string.Empty,
                                    Business = reader["Business"].ToString() ?? string.Empty,
                                };

                                var onboardingForAdmin = new OnboardingForAdmin
                                {
                                    ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0,
                                    BusinessId = reader["BusinessId"] != DBNull.Value ? Convert.ToInt32(reader["BusinessId"]) : 0,
                                    Ticket = reader["Ticket"].ToString() ?? string.Empty,
                                    Hierarchy = hierarchy,
                                    Platform = reader["Platform"].ToString() ?? string.Empty,
                                    ReceivedAt = reader["ReceivedAt"] != DBNull.Value ? Convert.ToDateTime(reader["ReceivedAt"]) : null,
                                    Status = reader["Status"] != DBNull.Value ? reader["Status"].ToString() : null,
                                    AssignedToId = reader["AssignedToId"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : null,
                                };

                                onboardingForAdmins.Add(onboardingForAdmin);
                            }

                            response.Data = onboardingForAdmins;
                            response.IsSuccess = true;
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                response.IsSuccess = false;
                response.ErrorMessage = $"SQL Error: {sqlEx.Message}";
                _logger.LogError($"SQL Error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = $"An error occurred: {ex.Message}";
                _logger.LogError($"An error occurred: {ex.Message}");
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> UpdateAssignedToId(AssignmentDTO assignmentDTO)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE zb.Onboarding " +
                                 "SET AssignedToId = @AssignedToId " +
                                 "WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AssignedToId", assignmentDTO.AssignedToId);
                        command.Parameters.AddWithValue("@ID", assignmentDTO.ID);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            response.Data = true;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "No records were updated. Record may not exist.";
                            _logger.LogError($"No record found to update with ID {assignmentDTO.ID}.");
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

        public async Task<ServiceResponse<List<OnboardingForSubAdmin>>> GetForSubAdmin(int assignedToId)
        {
            var response = new ServiceResponse<List<OnboardingForSubAdmin>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("OnboardingForSubAdmin", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@AssignedToId", assignedToId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            var onboardingForSubAdmins = new List<OnboardingForSubAdmin>();

                            while (await reader.ReadAsync())
                            {
                                var hierarchy = new Hierarchy
                                {
                                    Tenant = reader["Tenant"].ToString() ?? string.Empty,
                                    Customer = reader["Customer"].ToString() ?? string.Empty,
                                    Business = reader["Business"].ToString() ?? string.Empty,
                                };

                                var onboardingForSubAdmin = new OnboardingForSubAdmin
                                {
                                    ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0,
                                    BusinessId = reader["BusinessId"] != DBNull.Value ? Convert.ToInt32(reader["BusinessId"]) : 0,
                                    Ticket = reader["Ticket"].ToString() ?? string.Empty,
                                    Hierarchy = hierarchy,
                                    Platform = reader["Platform"].ToString() ?? string.Empty,
                                    ReceivedAt = reader["ReceivedAt"] != DBNull.Value ? Convert.ToDateTime(reader["ReceivedAt"]) : null,
                                    Status = reader["Status"] != DBNull.Value ? reader["Status"].ToString() : null,
                                    FileAddress = reader["FileAddress"] != DBNull.Value ? reader["FileAddress"].ToString() : null,
                                    Password = reader["Password"] != DBNull.Value ? reader["Password"].ToString() : null,
                                };

                                onboardingForSubAdmins.Add(onboardingForSubAdmin);
                            }

                            response.Data = onboardingForSubAdmins;
                            response.IsSuccess = true;
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                response.IsSuccess = false;
                response.ErrorMessage = $"SQL Error: {sqlEx.Message}";
                _logger.LogError($"SQL Error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = $"An error occurred: {ex.Message}";
                _logger.LogError($"An error occurred: {ex.Message}");
            }

            return response;
        }
    }
}