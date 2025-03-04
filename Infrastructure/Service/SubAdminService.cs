using Application.Interface;
using Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace Infrastructure.Service
{
    public class SubAdminService : ISubAdminService
    {
        private readonly string _connectionString;
        private readonly ILogger<SubAdminService> _logger;

        public SubAdminService(
            IConfiguration configuration,
            ILogger<SubAdminService> logger
        )
        {
            _connectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        public async Task<ServiceResponse<List<SubAdmin>>> GetAll()
        {
            var response = new ServiceResponse<List<SubAdmin>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.SubAdmin WHERE IsActive = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                    {
                        List<SubAdmin> subAdmins = new List<SubAdmin>();

                        while (await dataReader.ReadAsync())
                        {
                            SubAdmin subAdmin = new SubAdmin
                            {
                                ID = Convert.ToInt32(dataReader["ID"]),
                                AdminId = Convert.ToInt32(dataReader["AdminId"]),
                                ObjectId = dataReader["ObjectId"].ToString() ?? string.Empty,
                                Name = dataReader["Name"].ToString() ?? string.Empty,
                                Email = dataReader["Email"].ToString() ?? string.Empty,
                                Phone = dataReader["Phone"].ToString() ?? string.Empty,
                                Address = dataReader["Address"].ToString() ?? string.Empty,
                                Permissions = JsonConvert.DeserializeObject<SubAdminPermission>(dataReader["Permissions"].ToString() ?? "{}") ?? new SubAdminPermission(),
                                Status = dataReader["Status"].ToString() ?? string.Empty
                            };
                            subAdmins.Add(subAdmin);
                        }

                        response.Data = subAdmins;
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

        public async Task<ServiceResponse<SubAdmin>> GetByID(int id)
        {
            var response = new ServiceResponse<SubAdmin>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.SubAdmin WHERE ID = @ID AND IsActive = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                SubAdmin subAdmin = new SubAdmin
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    AdminId = Convert.ToInt32(dataReader["AdminId"]),
                                    ObjectId = dataReader["ObjectId"].ToString() ?? string.Empty,
                                    Name = dataReader["Name"].ToString() ?? string.Empty,
                                    Email = dataReader["Email"].ToString() ?? string.Empty,
                                    Phone = dataReader["Phone"].ToString() ?? string.Empty,
                                    Address = dataReader["Address"].ToString() ?? string.Empty,
                                    Permissions = JsonConvert.DeserializeObject<SubAdminPermission>(dataReader["Permissions"].ToString() ?? "{}") ?? new SubAdminPermission(),
                                    Status = dataReader["Status"].ToString() ?? string.Empty
                                };
                                response.Data = subAdmin;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = "SubAdmin not found.";
                                _logger.LogError("SubAdmin not found.");
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

        public async Task<ServiceResponse<SubAdmin>> GetByObjectID(string objectId)
        {
            var response = new ServiceResponse<SubAdmin>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.SubAdmin WHERE ObjectId = @ObjectId AND IsActive = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ObjectId", objectId);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                SubAdmin subAdmin = new SubAdmin
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    AdminId = Convert.ToInt32(dataReader["AdminId"]),
                                    ObjectId = dataReader["ObjectId"].ToString() ?? string.Empty,
                                    Name = dataReader["Name"].ToString() ?? string.Empty,
                                    Email = dataReader["Email"].ToString() ?? string.Empty,
                                    Phone = dataReader["Phone"].ToString() ?? string.Empty,
                                    Address = dataReader["Address"].ToString() ?? string.Empty,
                                    Permissions = JsonConvert.DeserializeObject<SubAdminPermission>(dataReader["Permissions"].ToString() ?? "{}") ?? new SubAdminPermission(),
                                    Status = dataReader["Status"].ToString() ?? string.Empty
                                };
                                response.Data = subAdmin;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = "SubAdmin not found.";
                                _logger.LogError("SubAdmin not found.");
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

        public async Task<ServiceResponse<List<SubAdmin>>> GetByAdminId(int adminId)
        {
            var response = new ServiceResponse<List<SubAdmin>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.SubAdmin WHERE AdminId = @AdminId AND IsActive = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AdminId", adminId);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            List<SubAdmin> subAdmins = new List<SubAdmin>();

                            while (await dataReader.ReadAsync())
                            {
                                SubAdmin subAdmin = new SubAdmin
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    AdminId = Convert.ToInt32(dataReader["AdminId"]),
                                    ObjectId = dataReader["ObjectId"].ToString() ?? string.Empty,
                                    Name = dataReader["Name"].ToString() ?? string.Empty,
                                    Email = dataReader["Email"].ToString() ?? string.Empty,
                                    Phone = dataReader["Phone"].ToString() ?? string.Empty,
                                    Address = dataReader["Address"].ToString() ?? string.Empty,
                                    Permissions = JsonConvert.DeserializeObject<SubAdminPermission>(dataReader["Permissions"].ToString() ?? "{}") ?? new SubAdminPermission(),
                                    Status = dataReader["Status"].ToString() ?? string.Empty
                                };
                                subAdmins.Add(subAdmin);
                            }

                            response.Data = subAdmins;
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

        public async Task<ServiceResponse<int?>> Post(SubAdmin subAdmin)
        {
            var response = new ServiceResponse<int?>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "INSERT INTO zb.SubAdmin (AdminId, ObjectId, Name, Email, Phone, Address, Permissions, Status) " +
                                 "VALUES (@AdminId, @ObjectId, @Name, @Email, @Phone, @Address, @Permissions, @Status); " +
                                 "SELECT SCOPE_IDENTITY()";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AdminId", subAdmin.AdminId);
                        command.Parameters.AddWithValue("@ObjectId", subAdmin.ObjectId);
                        command.Parameters.AddWithValue("@Name", subAdmin.Name);
                        command.Parameters.AddWithValue("@Email", subAdmin.Email);
                        command.Parameters.AddWithValue("@Phone", subAdmin.Phone);
                        command.Parameters.AddWithValue("@Address", subAdmin.Address);
                        command.Parameters.AddWithValue("@Permissions", JsonConvert.SerializeObject(subAdmin.Permissions));
                        command.Parameters.AddWithValue("@Status", subAdmin.Status);

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

        public async Task<ServiceResponse<bool>> Update(SubAdmin subAdmin)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE zb.SubAdmin SET AdminId = @AdminId, ObjectId = @ObjectId, Name = @Name, " +
                                 "Email = @Email, Phone = @Phone, Address = @Address, Permissions = @Permissions, Status = @Status " +
                                 "WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AdminId", subAdmin.AdminId);
                        command.Parameters.AddWithValue("@ObjectId", subAdmin.ObjectId);
                        command.Parameters.AddWithValue("@Name", subAdmin.Name);
                        command.Parameters.AddWithValue("@Email", subAdmin.Email);
                        command.Parameters.AddWithValue("@Phone", subAdmin.Phone);
                        command.Parameters.AddWithValue("@Address", subAdmin.Address);
                        command.Parameters.AddWithValue("@Permissions", JsonConvert.SerializeObject(subAdmin.Permissions));
                        command.Parameters.AddWithValue("@Status", subAdmin.Status);
                        command.Parameters.AddWithValue("@ID", subAdmin.ID);

                        int affectedRows = await command.ExecuteNonQueryAsync();
                        response.Data = affectedRows > 0;
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

                    string sql = "UPDATE zb.SubAdmin SET ObjectId = @ObjectId, Status = 'invited' WHERE Email = @Email";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ObjectId", objectId);
                        command.Parameters.AddWithValue("@Email", email);

                        int affectedRows = await command.ExecuteNonQueryAsync();
                        response.Data = affectedRows > 0;
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

                    string sql = "UPDATE zb.SubAdmin SET IsActive = 0 WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);

                        int affectedRows = await command.ExecuteNonQueryAsync();
                        response.Data = affectedRows > 0;
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