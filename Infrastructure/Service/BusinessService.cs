using Application.Interface;
using Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
	public class BusinessService : IBusinessService
	{
		private readonly string _connectionString;
		private readonly ILogger<BusinessService> _logger;

		public BusinessService(IConfiguration configuration, ILogger<BusinessService> logger)
		{
			_connectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
			_logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
		}

        public async Task<ServiceResponse<Hierarchy>> GetHierarchy(int businessId)
        {
            var response = new ServiceResponse<Hierarchy>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = @"
                        SELECT t.Name AS Tenant, c.Name AS Customer, b.Name AS Business
                        FROM zb.Business b
                        INNER JOIN zb.Customer c ON b.CustomerId = c.ID
                        INNER JOIN zb.Tenant t ON c.TenantId = t.ID
                        WHERE b.ID = @BusinessId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@BusinessId", businessId);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            if (await dataReader.ReadAsync())
                            {
                                Hierarchy hierarchy = new Hierarchy
                                {
                                    Tenant = dataReader["Tenant"].ToString() ?? string.Empty,
                                    Customer = dataReader["Customer"].ToString() ?? string.Empty,
                                    Business = dataReader["Business"].ToString() ?? string.Empty
                                };

                                response.Data = hierarchy;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.IsSuccess = false;
                                response.ErrorMessage = "No records found for the given BusinessId.";
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

        public async Task<ServiceResponse<List<Business>>> GetAll()
		{
			var response = new ServiceResponse<List<Business>>();
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					string sql = "SELECT * FROM zb.Business WHERE IsActive = 1";

					using (SqlCommand command = new SqlCommand(sql, connection))
					using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
					{
						List<Business> businesses = new List<Business>();

						while (await dataReader.ReadAsync())
						{
							Business business = new Business
							{
								ID = Convert.ToInt32(dataReader["ID"]),
								CustomerId = Convert.ToInt32(dataReader["CustomerId"]),
								Name = dataReader["Name"].ToString() ?? string.Empty,
								Email = dataReader["Email"].ToString() ?? string.Empty,
								Phone = dataReader["Phone"].ToString() ?? string.Empty,
								Industry = dataReader["Industry"].ToString() ?? string.Empty
							};
							businesses.Add(business);
						}

						response.Data = businesses;
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

        public async Task<ServiceResponse<List<Business>>> GetByTenantId(int tenantId)
        {
            var response = new ServiceResponse<List<Business>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = @"
						SELECT b.ID, b.CustomerId, b.Name, b.Email, b.Phone, b.Address
						FROM zb.Business b
						INNER JOIN zb.Customer c ON b.CustomerId = c.ID
						WHERE c.TenantId = @TenantId     
						AND b.IsActive = 1 
						AND c.IsActive = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@TenantId", tenantId);

                        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                        {
                            List<Business> businesses = new List<Business>();

                            while (await dataReader.ReadAsync())
                            {
                                Business business = new Business
                                {
                                    ID = Convert.ToInt32(dataReader["ID"]),
                                    CustomerId = Convert.ToInt32(dataReader["CustomerId"]),
                                    Name = dataReader["Name"].ToString() ?? string.Empty,
                                    Email = dataReader["Email"].ToString() ?? string.Empty,
                                    Phone = dataReader["Phone"].ToString() ?? string.Empty,
                                    Industry = dataReader["Industry"].ToString() ?? string.Empty
                                };
                                businesses.Add(business);
                            }

                            response.Data = businesses;
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

        public async Task<ServiceResponse<List<Business>>> GetByCustomerId(int customerId)
		{
			var response = new ServiceResponse<List<Business>>();
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					string sql = "SELECT * FROM zb.Business WHERE CustomerId = @CustomerId AND IsActive = 1";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@CustomerId", customerId);

						using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
						{
							List<Business> businesses = new List<Business>();

							while (await dataReader.ReadAsync())
							{
								Business business = new Business
								{
									ID = Convert.ToInt32(dataReader["ID"]),
									CustomerId = Convert.ToInt32(dataReader["CustomerId"]),
									Name = dataReader["Name"].ToString() ?? string.Empty,
									Email = dataReader["Email"].ToString() ?? string.Empty,
									Phone = dataReader["Phone"].ToString() ?? string.Empty,
									Industry = dataReader["Industry"].ToString() ?? string.Empty
								};
								businesses.Add(business);
							}

							response.Data = businesses;
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

		public async Task<ServiceResponse<Business>> GetById(int id)
		{
			var response = new ServiceResponse<Business>();
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					string sql = "SELECT * FROM zb.Business WHERE ID = @ID AND IsActive = 1";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@ID", id);

						using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
						{
							if (await dataReader.ReadAsync())
							{
								Business business = new Business
								{
									ID = Convert.ToInt32(dataReader["ID"]),
									CustomerId = Convert.ToInt32(dataReader["CustomerId"]),
									Name = dataReader["Name"].ToString() ?? string.Empty,
									Email = dataReader["Email"].ToString() ?? string.Empty,
									Phone = dataReader["Phone"].ToString() ?? string.Empty,
                                    Industry = dataReader["Industry"].ToString() ?? string.Empty
                                };
								response.Data = business;
								response.IsSuccess = true;
							}
							else
							{
								response.ErrorMessage = "Business not found.";
								_logger.LogError("Business not found.");
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

		public async Task<ServiceResponse<int?>> Post(Business business)
		{
			var response = new ServiceResponse<int?>();
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					string sql = "INSERT INTO zb.Business (CustomerId, Name, Email, Phone, Industry) " +
                                 "VALUES (@CustomerId, @Name, @Email, @Phone, @Industry); " +
								 "SELECT SCOPE_IDENTITY();";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@CustomerId", business.CustomerId);
						command.Parameters.AddWithValue("@Name", business.Name);
						command.Parameters.AddWithValue("@Email", business.Email);
						command.Parameters.AddWithValue("@Phone", business.Phone);
						command.Parameters.AddWithValue("@Industry", business.Industry);

						int? lastInsertedId = Convert.ToInt32(await command.ExecuteScalarAsync());

						if (lastInsertedId != null)
						{
							response.Data = lastInsertedId.Value;
							response.IsSuccess = true;
						}
						else
						{
							response.IsSuccess = false;
							response.ErrorMessage = "Failed to insert business.";
							_logger.LogError("Failed to insert business.");
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

		public async Task<ServiceResponse<bool>> Update(Business business)
		{
			var response = new ServiceResponse<bool>();
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					string sql = "UPDATE zb.Business SET CustomerId = @CustomerId, " +
                                 "Name = @Name, Email = @Email, Phone = @Phone, Industry = @Industry " +
								 "WHERE ID = @ID";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@CustomerId", business.CustomerId);
						command.Parameters.AddWithValue("@Name", business.Name);
						command.Parameters.AddWithValue("@Email", business.Email);
						command.Parameters.AddWithValue("@Phone", business.Phone);
						command.Parameters.AddWithValue("@Industry", business.Industry);
						command.Parameters.AddWithValue("@ID", business.ID);

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

					string sql = "UPDATE zb.Business SET IsActive = 0 WHERE ID = @ID";

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

