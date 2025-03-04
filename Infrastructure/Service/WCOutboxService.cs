using Application.Interface;
using Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
    public class WCOutboxService : IWCOutboxService
    {
        private readonly string _connectionString;
        private readonly ILogger<WCOutboxService> _logger;

        public WCOutboxService(IConfiguration configuration, ILogger<WCOutboxService> logger)
        {
            _connectionString = configuration.GetConnectionString("SAP") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        public async Task<ServiceResponse<List<WCOutbox>>> GetAll()
        {
            var response = new ServiceResponse<List<WCOutbox>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM dbo.WCOutbox";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                    {
                        var wcOutboxes = new List<WCOutbox>();

                        while (await dataReader.ReadAsync())
                        {
                            wcOutboxes.Add(new WCOutbox
                            {
                                ID = Convert.ToInt32(dataReader["ID"]),
                                Ticket = dataReader["Ticket"].ToString() ?? string.Empty,
                                UserName = dataReader["UserName"].ToString() ?? string.Empty,
                                Password = dataReader["Password"].ToString() ?? string.Empty,
                                CompanyFileName = dataReader["CompanyFileName"].ToString() ?? string.Empty,
                                CompanyFileLocation = dataReader["CompanyFileLocation"].ToString() ?? string.Empty,
                                Requests = JsonConvert.DeserializeObject<List<WCRequest>>(dataReader["Requests"].ToString() ?? "[]")!,
                                Status = dataReader["Status"].ToString() ?? string.Empty
                            });
                        }

                        response.Data = wcOutboxes;
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

        public async Task<ServiceResponse<WCOutbox>> GetById(int id)
        {
            var response = new ServiceResponse<WCOutbox>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM dbo.WCOutbox WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                response.Data = new WCOutbox
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    Ticket = dataReader["Ticket"].ToString() ?? string.Empty,
                                    UserName = dataReader["UserName"].ToString() ?? string.Empty,
                                    Password = dataReader["Password"].ToString() ?? string.Empty,
                                    CompanyFileName = dataReader["CompanyFileName"].ToString() ?? string.Empty,
                                    CompanyFileLocation = dataReader["CompanyFileLocation"].ToString() ?? string.Empty,
                                    Requests = JsonConvert.DeserializeObject<List<WCRequest>>(dataReader["Requests"].ToString() ?? "[]")!,
                                    Status = dataReader["Status"].ToString() ?? string.Empty
                                };
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = "WCOutbox not found.";
                                _logger.LogError("WCOutbox not found.");
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

        public async Task<ServiceResponse<int?>> Post(WCOutbox wcOutbox)
        {
            var response = new ServiceResponse<int?>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "INSERT INTO dbo.WCOutbox (Ticket, UserName, Password, CompanyFileName, CompanyFileLocation, Requests, Status) " +
                                 "VALUES (@Ticket, @UserName, @Password, @CompanyFileName, @CompanyFileLocation, @Requests, @Status); " +
                                 "SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Ticket", wcOutbox.Ticket);
                        command.Parameters.AddWithValue("@UserName", wcOutbox.UserName);
                        command.Parameters.AddWithValue("@Password", wcOutbox.Password);
                        command.Parameters.AddWithValue("@CompanyFileName", wcOutbox.CompanyFileName);
                        command.Parameters.AddWithValue("@CompanyFileLocation", wcOutbox.CompanyFileLocation);
                        command.Parameters.AddWithValue("@Requests", JsonConvert.SerializeObject(wcOutbox.Requests));
                        command.Parameters.AddWithValue("@Status", wcOutbox.Status);

                        int? lastInsertedId = Convert.ToInt32(await command.ExecuteScalarAsync());

                        response.Data = lastInsertedId;
                        response.IsSuccess = lastInsertedId.HasValue;
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

        public async Task<ServiceResponse<bool>> Update(WCOutbox wcOutbox)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE dbo.WCOutbox SET Ticket = @Ticket, UserName = @UserName, Password = @Password, " +
                                 "CompanyFileName = @CompanyFileName, CompanyFileLocation = @CompanyFileLocation, " +
                                 "Requests = @Requests, Status = @Status WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Ticket", wcOutbox.Ticket);
                        command.Parameters.AddWithValue("@UserName", wcOutbox.UserName);
                        command.Parameters.AddWithValue("@Password", wcOutbox.Password);
                        command.Parameters.AddWithValue("@CompanyFileName", wcOutbox.CompanyFileName);
                        command.Parameters.AddWithValue("@CompanyFileLocation", wcOutbox.CompanyFileLocation);
                        command.Parameters.AddWithValue("@Requests", JsonConvert.SerializeObject(wcOutbox.Requests));
                        command.Parameters.AddWithValue("@Status", wcOutbox.Status);
                        command.Parameters.AddWithValue("@ID", wcOutbox.ID);

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

                    string sql = "DELETE FROM dbo.WCOutbox WHERE ID = @ID";

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