using Application.Interface;
using Core.Model;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
	public class TenantService : ITenantService
	{
		private readonly string _connectionString;
		private readonly ILogger<TenantService> _logger;

		public TenantService(
			IConfiguration configuration,
			ILogger<TenantService> logger
		)
		{
			_connectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
			_logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
		}

		public async Task<ServiceResponse<List<Tenant>>> GetAll()
		{
			var response = new ServiceResponse<List<Tenant>>();
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					string sql = "SELECT * FROM zb.Tenant WHERE IsActive = 1";

					using (SqlCommand command = new SqlCommand(sql, connection))
					using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
					{
						List<Tenant> tenants = new List<Tenant>();

						while (await dataReader.ReadAsync())
						{
							Tenant tenant = new Tenant
							{
								ID = Convert.ToInt32(dataReader["ID"]),
								ObjectId = dataReader["ObjectId"].ToString() ?? string.Empty,
								Name = dataReader["Name"].ToString() ?? string.Empty,
								Email = dataReader["Email"].ToString() ?? string.Empty,
								Phone = dataReader["Phone"].ToString() ?? string.Empty,
								Address = dataReader["Address"].ToString() ?? string.Empty,
								CompanyName = dataReader["CompanyName"].ToString() ?? string.Empty,
								CompanyEmail = dataReader["CompanyEmail"].ToString() ?? string.Empty,
								CompanyPhone = dataReader["CompanyPhone"].ToString() ?? string.Empty,
								CompanyAddress = dataReader["CompanyAddress"].ToString() ?? string.Empty,
								Website = dataReader["Website"].ToString() ?? string.Empty,
								Products = JsonConvert.DeserializeObject<List<string>>(dataReader["Products"].ToString() ?? string.Empty) ?? new List<string>(),
								Status = dataReader["Status"].ToString() ?? string.Empty,
                                PaymentMethod = dataReader["PaymentMethod"] != DBNull.Value ? dataReader["PaymentMethod"].ToString() : null,
                                TeamMember = dataReader["TeamMember"] != DBNull.Value ? dataReader["TeamMember"].ToString() : null
                            };
							tenants.Add(tenant);
						}

						response.Data = tenants;
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

		public async Task<ServiceResponse<Tenant>> GetByID(int id)
		{
			var response = new ServiceResponse<Tenant>();
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					string sql = "SELECT * FROM zb.Tenant WHERE ID = @ID AND IsActive = 1";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@ID", id);

						using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
						{
							if (await dataReader.ReadAsync())
							{
								Tenant tenant = new Tenant
								{
									ID = Convert.ToInt32(dataReader["ID"]),
									ObjectId = dataReader["ObjectId"].ToString() ?? string.Empty,
									Name = dataReader["Name"].ToString() ?? string.Empty,
									Email = dataReader["Email"].ToString() ?? string.Empty,
									Phone = dataReader["Phone"].ToString() ?? string.Empty,
									Address = dataReader["Address"].ToString() ?? string.Empty,
									CompanyName = dataReader["CompanyName"].ToString() ?? string.Empty,
									CompanyEmail = dataReader["CompanyEmail"].ToString() ?? string.Empty,
									CompanyPhone = dataReader["CompanyPhone"].ToString() ?? string.Empty,
									CompanyAddress = dataReader["CompanyAddress"].ToString() ?? string.Empty,
									Website = dataReader["Website"].ToString() ?? string.Empty,
									Products = JsonConvert.DeserializeObject<List<string>>(dataReader["Products"].ToString() ?? string.Empty) ?? new List<string>(),
									Status = dataReader["Status"].ToString() ?? string.Empty,
                                    PaymentMethod = dataReader["PaymentMethod"] != DBNull.Value ? dataReader["PaymentMethod"].ToString() : null,
                                    TeamMember = dataReader["TeamMember"] != DBNull.Value ? dataReader["TeamMember"].ToString() : null
                                };
								response.Data = tenant;
								response.IsSuccess = true;
							}
							else
							{
								response.ErrorMessage = "Tenant not found.";
								_logger.LogError("Tenant not found.");
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

		public async Task<ServiceResponse<Tenant>> GetByObjectID(string objectId)
		{
			var response = new ServiceResponse<Tenant>();
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					string sql = "SELECT * FROM zb.Tenant WHERE ObjectId = @ObjectId AND IsActive = 1";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@ObjectId", objectId);

						using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
						{
							if (await dataReader.ReadAsync())
							{
								Tenant tenant = new Tenant
								{
									ID = Convert.ToInt32(dataReader["ID"]),
									ObjectId = dataReader["ObjectId"].ToString() ?? string.Empty,
									Name = dataReader["Name"].ToString() ?? string.Empty,
									Email = dataReader["Email"].ToString() ?? string.Empty,
									Phone = dataReader["Phone"].ToString() ?? string.Empty,
									Address = dataReader["Address"].ToString() ?? string.Empty,
									CompanyName = dataReader["CompanyName"].ToString() ?? string.Empty,
									CompanyEmail = dataReader["CompanyEmail"].ToString() ?? string.Empty,
									CompanyPhone = dataReader["CompanyPhone"].ToString() ?? string.Empty,
									CompanyAddress = dataReader["CompanyAddress"].ToString() ?? string.Empty,
									Website = dataReader["Website"].ToString() ?? string.Empty,
									Products = JsonConvert.DeserializeObject<List<string>>(dataReader["Products"].ToString() ?? string.Empty) ?? new List<string>(),
									Status = dataReader["Status"].ToString() ?? string.Empty,
                                    PaymentMethod = dataReader["PaymentMethod"] != DBNull.Value ? dataReader["PaymentMethod"].ToString() : null,
                                    TeamMember = dataReader["TeamMember"] != DBNull.Value ? dataReader["TeamMember"].ToString() : null
                                };
								response.Data = tenant;
								response.IsSuccess = true;
							}
							else
							{
								response.ErrorMessage = "Tenant not found.";
								_logger.LogError("Tenant not found.");
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

		public async Task<ServiceResponse<int?>> Post(Tenant tenant)
		{
			var response = new ServiceResponse<int?>();
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					string sql = "INSERT INTO zb.Tenant (ObjectId, Name, Email, Phone, Address, CompanyName, CompanyEmail, CompanyPhone, " +
								 "CompanyAddress, Website, Products, Status) " +
								 "VALUES (@ObjectId, @Name, @Email, @Phone, @Address, @CompanyName, @CompanyEmail, @CompanyPhone, " +
								 "@CompanyAddress, @Website, @Products, @Status); " +
								 "SELECT SCOPE_IDENTITY();";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@ObjectId", tenant.ObjectId);
						command.Parameters.AddWithValue("@Name", tenant.Name);
						command.Parameters.AddWithValue("@Email", tenant.Email);
						command.Parameters.AddWithValue("@Phone", tenant.Phone);
						command.Parameters.AddWithValue("@Address", tenant.Address);
						command.Parameters.AddWithValue("@CompanyName", tenant.CompanyName);
						command.Parameters.AddWithValue("@CompanyEmail", tenant.CompanyEmail);
						command.Parameters.AddWithValue("@CompanyPhone", tenant.CompanyPhone);
						command.Parameters.AddWithValue("@CompanyAddress", tenant.CompanyAddress);
						command.Parameters.AddWithValue("@Website", tenant.Website);
						command.Parameters.AddWithValue("@Products", JsonConvert.SerializeObject(tenant.Products));
						command.Parameters.AddWithValue("@Status", tenant.Status);
                        //command.Parameters.AddWithValue("@PaymentMethod", string.IsNullOrEmpty(tenant.PaymentMethod) ? (object)DBNull.Value : tenant.PaymentMethod);
                        //command.Parameters.AddWithValue("@TeamMember", string.IsNullOrEmpty(tenant.TeamMember) ? (object)DBNull.Value : tenant.TeamMember);

                        int? lastInsertedId = Convert.ToInt32(await command.ExecuteScalarAsync());

						if (lastInsertedId != null)
						{
							response.Data = lastInsertedId.Value;
							response.IsSuccess = true;
						}
						else
						{
							response.IsSuccess = false;
							response.ErrorMessage = "Failed to insert tenant.";
							_logger.LogError("Failed to insert tenant.");
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

		public async Task<ServiceResponse<bool>> Update(Tenant tenant)
		{
			var response = new ServiceResponse<bool>();
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					string sql = "UPDATE zb.Tenant SET ObjectId = @ObjectId, Name = @Name, Email = @Email, Phone = @Phone, " +
								 "Address = @Address, CompanyName = @CompanyName, CompanyEmail = @CompanyEmail, " +
								 "CompanyPhone = @CompanyPhone, CompanyAddress = @CompanyAddress, Website = @Website, " +
								 "Products = @Products, Status = @Status " +
								 "WHERE ID = @ID";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@ObjectId", tenant.ObjectId);
						command.Parameters.AddWithValue("@Name", tenant.Name);
						command.Parameters.AddWithValue("@Email", tenant.Email);
						command.Parameters.AddWithValue("@Phone", tenant.Phone);
						command.Parameters.AddWithValue("@Address", tenant.Address);
						command.Parameters.AddWithValue("@CompanyName", tenant.CompanyName);
						command.Parameters.AddWithValue("@CompanyEmail", tenant.CompanyEmail);
						command.Parameters.AddWithValue("@CompanyPhone", tenant.CompanyPhone);
						command.Parameters.AddWithValue("@CompanyAddress", tenant.CompanyAddress);
						command.Parameters.AddWithValue("@Website", tenant.Website);
						command.Parameters.AddWithValue("@Products", JsonConvert.SerializeObject(tenant.Products));
						command.Parameters.AddWithValue("@Status", tenant.Status);
						command.Parameters.AddWithValue("@ID", tenant.ID);

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

        public async Task<ServiceResponse<bool>> UpdatePaymentMethod(int tenantId, string paymentMethod)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE zb.Tenant SET PaymentMethod = @paymentMethod WHERE ID = @tenantId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@tenantId", tenantId);
                        command.Parameters.AddWithValue("@paymentMethod", paymentMethod);

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

        public async Task<ServiceResponse<bool>> UpdateTeamMember(int tenantId, string teamMember)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE zb.Tenant SET TeamMember = @teamMember WHERE ID = @tenantId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@tenantId", tenantId);
                        command.Parameters.AddWithValue("@teamMember", teamMember);

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

					string sql = "UPDATE zb.Tenant SET ObjectId = @ObjectId, Status = 'invited' WHERE Email = @Email";

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

					string sql = "UPDATE zb.Tenant SET IsActive = 0 WHERE ID = @ID";

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