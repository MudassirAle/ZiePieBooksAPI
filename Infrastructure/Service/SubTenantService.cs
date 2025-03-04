using Application.Interface;
using Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace Infrastructure.Service
{
    public class SubTenantService : ISubTenantService
    {
        private readonly string _connectionString;
        private readonly ILogger<SubTenantService> _logger;

        public SubTenantService(
            IConfiguration configuration,
            ILogger<SubTenantService> logger
        )
        {
            _connectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        public async Task<ServiceResponse<List<SubTenant>>> GetAll()
        {
            var response = new ServiceResponse<List<SubTenant>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.SubTenant WHERE IsActive = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                    {
                        List<SubTenant> subTenants = new List<SubTenant>();

                        while (await dataReader.ReadAsync())
                        {
                            SubTenant subTenant = new SubTenant
                            {
                                ID = Convert.ToInt32(dataReader["ID"]),
                                TenantId = Convert.ToInt32(dataReader["TenantId"]),
                                ObjectId = dataReader["ObjectId"].ToString() ?? string.Empty,
                                Name = dataReader["Name"].ToString() ?? string.Empty,
                                Email = dataReader["Email"].ToString() ?? string.Empty,
                                Phone = dataReader["Phone"].ToString() ?? string.Empty,
                                Address = dataReader["Address"].ToString() ?? string.Empty,
                                Permissions = JsonConvert.DeserializeObject<SubTenantPermission>(dataReader["Permissions"].ToString() ?? "{}") ?? new SubTenantPermission(),
                                Status = dataReader["Status"].ToString() ?? string.Empty
                            };
                            subTenants.Add(subTenant);
                        }

                        response.Data = subTenants;
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

        public async Task<ServiceResponse<SubTenant>> GetByID(int id)
        {
            var response = new ServiceResponse<SubTenant>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.SubTenant WHERE ID = @ID  AND IsActive = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                SubTenant subTenant = new SubTenant
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    TenantId = Convert.ToInt32(dataReader["TenantId"]),
                                    ObjectId = dataReader["ObjectId"].ToString() ?? string.Empty,
                                    Name = dataReader["Name"].ToString() ?? string.Empty,
                                    Email = dataReader["Email"].ToString() ?? string.Empty,
                                    Phone = dataReader["Phone"].ToString() ?? string.Empty,
                                    Address = dataReader["Address"].ToString() ?? string.Empty,
                                    Permissions = JsonConvert.DeserializeObject<SubTenantPermission>(dataReader["Permissions"].ToString() ?? "{}") ?? new SubTenantPermission(),
                                    Status = dataReader["Status"].ToString() ?? string.Empty
                                };
                                response.Data = subTenant;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = "SubTenant not found.";
                                _logger.LogError("SubTenant not found.");
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

        public async Task<ServiceResponse<List<SubTenant>>> GetByTenantId(int tenantId)
        {
            var response = new ServiceResponse<List<SubTenant>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.SubTenant WHERE TenantId = @TenantId  AND IsActive = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@TenantId", tenantId);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            List<SubTenant> subTenants = new List<SubTenant>();

                            while (await dataReader.ReadAsync())
                            {
                                SubTenant subTenant = new SubTenant
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    TenantId = Convert.ToInt32(dataReader["TenantId"]),
                                    ObjectId = dataReader["ObjectId"].ToString() ?? string.Empty,
                                    Name = dataReader["Name"].ToString() ?? string.Empty,
                                    Email = dataReader["Email"].ToString() ?? string.Empty,
                                    Phone = dataReader["Phone"].ToString() ?? string.Empty,
                                    Address = dataReader["Address"].ToString() ?? string.Empty,
                                    Permissions = JsonConvert.DeserializeObject<SubTenantPermission>(dataReader["Permissions"].ToString() ?? "{}") ?? new SubTenantPermission(),
                                    Status = dataReader["Status"].ToString() ?? string.Empty
                                };
                                subTenants.Add(subTenant);
                            }

                            response.Data = subTenants;
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

        public async Task<ServiceResponse<SubTenant>> GetByObjectID(string objectId)
        {
            var response = new ServiceResponse<SubTenant>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.SubTenant WHERE ObjectId = @ObjectId  AND IsActive = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ObjectId", objectId);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                SubTenant subTenant = new SubTenant
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    TenantId = Convert.ToInt32(dataReader["TenantId"]),
                                    ObjectId = dataReader["ObjectId"].ToString() ?? string.Empty,
                                    Name = dataReader["Name"].ToString() ?? string.Empty,
                                    Email = dataReader["Email"].ToString() ?? string.Empty,
                                    Phone = dataReader["Phone"].ToString() ?? string.Empty,
                                    Address = dataReader["Address"].ToString() ?? string.Empty,
                                    Permissions = JsonConvert.DeserializeObject<SubTenantPermission>(dataReader["Permissions"].ToString() ?? "{}") ?? new SubTenantPermission(),
                                    Status = dataReader["Status"].ToString() ?? string.Empty
                                };
                                response.Data = subTenant;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = "SubTenant not found.";
                                _logger.LogError("SubTenant not found.");
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

        public async Task<ServiceResponse<int?>> Post(SubTenant subTenant)
        {
            var response = new ServiceResponse<int?>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "INSERT INTO zb.SubTenant (TenantId, ObjectId, Name, Email, Phone, Address, Permissions, Status) " +
                                 "VALUES (@TenantId, @ObjectId, @Name, @Email, @Phone, @Address, @Permissions, @Status); " +
                                 "SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@TenantId", subTenant.TenantId);
                        command.Parameters.AddWithValue("@ObjectId", subTenant.ObjectId);
                        command.Parameters.AddWithValue("@Name", subTenant.Name);
                        command.Parameters.AddWithValue("@Email", subTenant.Email);
                        command.Parameters.AddWithValue("@Phone", subTenant.Phone);
                        command.Parameters.AddWithValue("@Address", subTenant.Address);
                        command.Parameters.AddWithValue("@Permissions", JsonConvert.SerializeObject(subTenant.Permissions));
                        command.Parameters.AddWithValue("@Status", subTenant.Status);

                        var result = await command.ExecuteScalarAsync();
                        response.Data = Convert.ToInt32(result);
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

        public async Task<ServiceResponse<bool>> Update(SubTenant subTenant)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE zb.SubTenant SET TenantId = @TenantId, ObjectId = @ObjectId, " +
                                 "Name = @Name, Email = @Email, Phone = @Phone, Address = @Address, Permissions = @Permissions, Status = @Status " +
                                 "WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@TenantId", subTenant.TenantId);
                        command.Parameters.AddWithValue("@ObjectId", subTenant.ObjectId);
                        command.Parameters.AddWithValue("@Name", subTenant.Name);
                        command.Parameters.AddWithValue("@Email", subTenant.Email);
                        command.Parameters.AddWithValue("@Phone", subTenant.Phone);
                        command.Parameters.AddWithValue("@Address", subTenant.Address);
                        command.Parameters.AddWithValue("@Permissions", JsonConvert.SerializeObject(subTenant.Permissions));
                        command.Parameters.AddWithValue("@Status", subTenant.Status);
                        command.Parameters.AddWithValue("@ID", subTenant.ID);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        response.Data = rowsAffected > 0;
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

        public async Task<ServiceResponse<bool>> UpdateObjectId(string email, string objectId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE zb.SubTenant SET ObjectId = @ObjectId, Status = 'Invited' WHERE Email = @Email";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ObjectId", objectId);
                        command.Parameters.AddWithValue("@Email", email);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        response.Data = rowsAffected > 0;
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

        public async Task<ServiceResponse<bool>> Delete(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE zb.SubTenant SET IsActive = 0 WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        response.Data = rowsAffected > 0;
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
    }
}

