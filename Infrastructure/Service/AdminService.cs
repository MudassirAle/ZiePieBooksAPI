using Application.Interface;
using Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace Infrastructure.Service
{
    public class AdminService : IAdminService
    {
        private readonly string _connectionString;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            IConfiguration configuration,
            ILogger<AdminService> logger
        )
        {
            _connectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        public async Task<ServiceResponse<List<Admin>>> GetAll()
        {
            var response = new ServiceResponse<List<Admin>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.Admin WHERE IsActive = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                    {
                        List<Admin> admins = new List<Admin>();

                        while (await dataReader.ReadAsync())
                        {
                            Admin admin = new Admin
                            {
                                ID = Convert.ToInt32(dataReader["ID"]),
                                ObjectId = dataReader["ObjectId"].ToString() ?? string.Empty,
                                Name = dataReader["Name"].ToString() ?? string.Empty,
                                Email = dataReader["Email"].ToString() ?? string.Empty,
                                Phone = dataReader["Phone"].ToString() ?? string.Empty,
                                Address = dataReader["Address"].ToString() ?? string.Empty,
                                Permissions = JsonConvert.DeserializeObject<List<string>>(dataReader["Permissions"].ToString() ?? string.Empty) ?? new List<string>(),
                                Status = dataReader["Status"].ToString() ?? string.Empty
                            };
                            admins.Add(admin);
                        }

                        response.Data = admins;
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

        public async Task<ServiceResponse<Admin>> GetByID(int id)
        {
            var response = new ServiceResponse<Admin>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.Admin WHERE ID = @ID AND IsActive = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                Admin admin = new Admin
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    ObjectId = dataReader["ObjectId"].ToString() ?? string.Empty,
                                    Name = dataReader["Name"].ToString() ?? string.Empty,
                                    Email = dataReader["Email"].ToString() ?? string.Empty,
                                    Phone = dataReader["Phone"].ToString() ?? string.Empty,
                                    Address = dataReader["Address"].ToString() ?? string.Empty,
                                    Permissions = JsonConvert.DeserializeObject<List<string>>(dataReader["Permissions"].ToString() ?? string.Empty) ?? new List<string>(),
                                    Status = dataReader["Status"].ToString() ?? string.Empty
                                };
                                response.Data = admin;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = "Admin not found.";
                                _logger.LogError("Admin not found.");
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

        public async Task<ServiceResponse<Admin>> GetByObjectID(string objectId)
        {
            var response = new ServiceResponse<Admin>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.Admin WHERE ObjectId = @ObjectId AND IsActive = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ObjectId", objectId);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                Admin admin = new Admin
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    ObjectId = dataReader["ObjectId"].ToString() ?? string.Empty,
                                    Name = dataReader["Name"].ToString() ?? string.Empty,
                                    Email = dataReader["Email"].ToString() ?? string.Empty,
                                    Phone = dataReader["Phone"].ToString() ?? string.Empty,
                                    Address = dataReader["Address"].ToString() ?? string.Empty,
                                    Permissions = JsonConvert.DeserializeObject<List<string>>(dataReader["Permissions"].ToString() ?? string.Empty) ?? new List<string>(),
                                    Status = dataReader["Status"].ToString() ?? string.Empty
                                };
                                response.Data = admin;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = "Admin not found.";
                                _logger.LogError("Admin not found.");
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

        public async Task<ServiceResponse<int?>> Post(Admin admin)
        {
            var response = new ServiceResponse<int?>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "INSERT INTO zb.Admin (ObjectId, Name, Email, Phone, Address, Permissions, Status) " +
                                 "VALUES (@ObjectId, @Name, @Email, @Phone, @Address, @Permissions, @Status); " +
                                 "SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ObjectId", admin.ObjectId);
                        command.Parameters.AddWithValue("@Name", admin.Name);
                        command.Parameters.AddWithValue("@Email", admin.Email);
                        command.Parameters.AddWithValue("@Phone", admin.Phone);
                        command.Parameters.AddWithValue("@Address", admin.Address);
                        command.Parameters.AddWithValue("@Permissions", JsonConvert.SerializeObject(admin.Permissions));
                        command.Parameters.AddWithValue("@Status", admin.Status);

                        int? lastInsertedId = Convert.ToInt32(await command.ExecuteScalarAsync());

                        if (lastInsertedId != null)
                        {
                            response.Data = lastInsertedId.Value;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Failed to insert admin.";
                            _logger.LogError("Failed to insert admin.");
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

        public async Task<ServiceResponse<bool>> Update(Admin admin)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE zb.Admin SET ObjectId = @ObjectId, Name = @Name, Email = @Email, Phone = @Phone, " +
                                 "Address = @Address, Permissions = @Permissions, Status = @Status " +
                                 "WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ObjectId", admin.ObjectId);
                        command.Parameters.AddWithValue("@Name", admin.Name);
                        command.Parameters.AddWithValue("@Email", admin.Email);
                        command.Parameters.AddWithValue("@Phone", admin.Phone);
                        command.Parameters.AddWithValue("@Address", admin.Address);
                        command.Parameters.AddWithValue("@Permissions", JsonConvert.SerializeObject(admin.Permissions));
                        command.Parameters.AddWithValue("@Status", admin.Status);
                        command.Parameters.AddWithValue("@ID", admin.ID);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            response.Data = true;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Failed to update admin.";
                            _logger.LogError("Failed to update admin.");
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

        public async Task<ServiceResponse<bool>> UpdateObjectId(string email, string objectId)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE zb.Admin SET ObjectId = @ObjectId, Status = 'invited' WHERE Email = @Email";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ObjectId", objectId);
                        command.Parameters.AddWithValue("@Email", email);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            response.Data = true;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Failed to update ObjectId.";
                            _logger.LogError("Failed to update ObjectId.");
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

                    string sql = "Update zb.Admin SET IsActive = 0 WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            response.Data = true;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Failed to delete admin.";
                            _logger.LogError("Failed to delete admin.");
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
