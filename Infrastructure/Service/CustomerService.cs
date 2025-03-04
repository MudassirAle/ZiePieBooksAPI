using Application.Interface;
using Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace Infrastructure.Service
{
	public class CustomerService : ICustomerService
	{
		private readonly string _connectionString;
		private readonly ILogger<CustomerService> _logger;

		public CustomerService(
			IConfiguration configuration,
			ILogger<CustomerService> logger
		)
		{
			_connectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
			_logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
		}

		public async Task<ServiceResponse<List<Customer>>> GetAll()
		{
			var response = new ServiceResponse<List<Customer>>();
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					string sql = "SELECT * FROM zb.Customer WHERE IsActive = 1"; // Adjust SQL query as per your database schema

					using (SqlCommand command = new SqlCommand(sql, connection))
					using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
					{
						List<Customer> customers = new List<Customer>();

						while (await dataReader.ReadAsync())
						{
							Customer customer = new Customer
							{
								ID = Convert.ToInt32(dataReader["ID"]),
								TenantId = Convert.ToInt32(dataReader["TenantId"]),
								ObjectId = dataReader["ObjectId"].ToString() ?? string.Empty,
								Name = dataReader["Name"].ToString() ?? string.Empty,
								Email = dataReader["Email"].ToString() ?? string.Empty,
								Phone = dataReader["Phone"].ToString() ?? string.Empty,
                                Service = dataReader["Service"].ToString() ?? string.Empty,
                                Status = dataReader["Status"].ToString() ?? string.Empty,
                                Platform = dataReader["Platform"].ToString() ?? string.Empty,
                                PlaidConnectedBy = dataReader["PlaidConnectedBy"].ToString() ?? string.Empty
                            };
							customers.Add(customer);
						}

						response.Data = customers;
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

		public async Task<ServiceResponse<Customer>> GetById(int id)
		{
			var response = new ServiceResponse<Customer>();
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					string sql = "SELECT * FROM zb.Customer WHERE ID = @ID AND IsActive = 1";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@ID", id);

						using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
						{
							if (await dataReader.ReadAsync())
							{
								Customer customer = new Customer
								{
									ID = Convert.ToInt32(dataReader["ID"]),
									TenantId = Convert.ToInt32(dataReader["TenantId"]),
									ObjectId = dataReader["ObjectId"].ToString() ?? string.Empty,
									Name = dataReader["Name"].ToString() ?? string.Empty,
									Email = dataReader["Email"].ToString() ?? string.Empty,
									Phone = dataReader["Phone"].ToString() ?? string.Empty,
									Service = dataReader["Service"].ToString() ?? string.Empty,
									Status = dataReader["Status"].ToString() ?? string.Empty,
                                    Platform = dataReader["Platform"].ToString() ?? string.Empty,
                                    PlaidConnectedBy = dataReader["PlaidConnectedBy"].ToString() ?? string.Empty
                                };
								response.Data = customer;
								response.IsSuccess = true;
							}
							else
							{
								response.ErrorMessage = "Customer not found.";
								_logger.LogError("Customer not found.");
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

		public async Task<ServiceResponse<List<Customer>>> GetByTenantId(int tenantId)
		{
			var response = new ServiceResponse<List<Customer>>();
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					string sql = "SELECT * FROM zb.Customer WHERE TenantId = @TenantId AND IsActive = 1";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@TenantId", tenantId);

						using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
						{
							List<Customer> customers = new List<Customer>();

							while (await dataReader.ReadAsync())
							{
								Customer customer = new Customer
								{
									ID = Convert.ToInt32(dataReader["ID"]),
									TenantId = Convert.ToInt32(dataReader["TenantId"]),
									ObjectId = dataReader["ObjectId"].ToString() ?? string.Empty,
									Name = dataReader["Name"].ToString() ?? string.Empty,
									Email = dataReader["Email"].ToString() ?? string.Empty,
									Phone = dataReader["Phone"].ToString() ?? string.Empty,
                                    Service = dataReader["Service"].ToString() ?? string.Empty,
									Status = dataReader["Status"].ToString() ?? string.Empty,
                                    Platform = dataReader["Platform"].ToString() ?? string.Empty,
                                    PlaidConnectedBy = dataReader["PlaidConnectedBy"].ToString() ?? string.Empty
                                };
								customers.Add(customer);
							}

							response.Data = customers;
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

        public async Task<ServiceResponse<Customer>> GetByObjectId(string objectId)
        {
            var response = new ServiceResponse<Customer>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.Customer WHERE ObjectId = @ObjectId AND IsActive = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ObjectId", objectId);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                Customer customer = new Customer
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    TenantId = Convert.ToInt32(dataReader["TenantId"]),
                                    ObjectId = dataReader["ObjectId"].ToString() ?? string.Empty,
                                    Name = dataReader["Name"].ToString() ?? string.Empty,
                                    Email = dataReader["Email"].ToString() ?? string.Empty,
                                    Phone = dataReader["Phone"].ToString() ?? string.Empty,
                                    Service = dataReader["Service"].ToString() ?? string.Empty,
                                    Status = dataReader["Status"].ToString() ?? string.Empty,
                                    Platform = dataReader["Platform"].ToString() ?? string.Empty,
                                    PlaidConnectedBy = dataReader["PlaidConnectedBy"].ToString() ?? string.Empty
                                };
                                response.Data = customer;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = "Customer not found.";
                                _logger.LogError("Customer not found.");
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

        public async Task<ServiceResponse<int?>> Post(Customer customer)
        {
            var response = new ServiceResponse<int?>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "INSERT INTO zb.Customer (TenantId, ObjectId, Name, Email, Phone, Service, Status, Platform, PlaidConnectedBy) " +
                                 "VALUES (@TenantId, @ObjectId, @Name, @Email, @Phone, @Service, @Status, @Platform, @PlaidConnectedBy); " +
                                 "SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@TenantId", customer.TenantId);
                        command.Parameters.AddWithValue("@ObjectId", customer.ObjectId);
                        command.Parameters.AddWithValue("@Name", customer.Name);
                        command.Parameters.AddWithValue("@Email", customer.Email);
                        command.Parameters.AddWithValue("@Phone", customer.Phone);
                        command.Parameters.AddWithValue("@Service", customer.Service);
                        command.Parameters.AddWithValue("@Status", customer.Status);
                        command.Parameters.AddWithValue("@Platform", customer.Platform);
                        command.Parameters.AddWithValue("@PlaidConnectedBy", customer.PlaidConnectedBy);

                        int? lastInsertedId = Convert.ToInt32(await command.ExecuteScalarAsync());

                        if (lastInsertedId != null)
                        {
                            response.Data = lastInsertedId.Value;
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Failed to insert customer.";
                            _logger.LogError("Failed to insert customer.");
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

        public async Task<ServiceResponse<bool>> Update(Customer customer)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE zb.Customer SET TenantId = @TenantId, ObjectId = @ObjectId, " +
                                 "Name = @Name, Email = @Email, Phone = @Phone, Service = @Service, " +
                                 "Status = @Status, Platform = @Platform, PlaidConnectedBy = @PlaidConnectedBy " +
                                 "WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@TenantId", customer.TenantId);
                        command.Parameters.AddWithValue("@ObjectId", customer.ObjectId);
                        command.Parameters.AddWithValue("@Name", customer.Name);
                        command.Parameters.AddWithValue("@Email", customer.Email);
                        command.Parameters.AddWithValue("@Phone", customer.Phone);
                        command.Parameters.AddWithValue("@Service", customer.Service);
                        command.Parameters.AddWithValue("@Status", customer.Status);
                        command.Parameters.AddWithValue("@Platform", customer.Platform);
                        command.Parameters.AddWithValue("@PlaidConnectedBy", customer.PlaidConnectedBy);
                        command.Parameters.AddWithValue("@ID", customer.ID);

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

					string sql = "UPDATE zb.Customer SET ObjectId = @ObjectId, Status = 'invited' WHERE Email = @Email";

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

					string sql = "UPDATE zb.Customer SET IsActive = 0 WHERE ID = @ID";

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