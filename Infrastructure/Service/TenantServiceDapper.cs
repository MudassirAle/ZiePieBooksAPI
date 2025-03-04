//using Application.Interface;
//using Core.Model;
//using Dapper;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using System.Data.SqlClient;
//using System.Threading.Tasks;

//namespace Infrastructure.Service
//{
//	public class TenantService : ITenantService
//	{
//		private readonly string _connectionString;
//		private readonly ILogger<TenantService> _logger;

//		public TenantService(
//			IConfiguration configuration,
//			ILogger<TenantService> logger
//		)
//		{
//			_connectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
//			_logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
//		}

//		public async Task<ServiceResponse<List<Tenant>>> GetAll()
//		{
//			var response = new ServiceResponse<List<Tenant>>();
//			try
//			{
//				using (var connection = new SqlConnection(_connectionString))
//				{
//					var tenants = await connection.QueryAsync<Tenant>("SELECT * FROM zb.Tenants");

//					if (tenants.AsList().Count > 0)
//					{
//						response.Data = tenants.AsList();
//						response.IsSuccess = true;
//					}
//					else
//					{
//						response.IsSuccess = false;
//						response.ErrorMessage = "No tenants found.";
//					}
//				}
//			}
//			catch (SqlException sqlEx)
//			{
//				HandleSqlException(response, sqlEx);
//			}
//			catch (Exception ex)
//			{
//				HandleGeneralException(response, ex);
//			}
//			return response;
//		}

//		public async Task<ServiceResponse<Tenant>> GetByObjectID(string objectId)
//		{
//			var response = new ServiceResponse<Tenant>();
//			try
//			{
//				using (var connection = new SqlConnection(_connectionString))
//				{
//					var tenant = await connection.QueryFirstOrDefaultAsync<Tenant>("SELECT * FROM zb.Tenants WHERE ObjectId = @ObjectId", new { ObjectId = objectId });

//					if (tenant != null)
//					{
//						response.Data = tenant;
//						response.IsSuccess = true;
//					}
//					else
//					{
//						response.IsSuccess = false;
//						response.ErrorMessage = "Tenant not found.";
//					}
//				}
//			}
//			catch (SqlException sqlEx)
//			{
//				HandleSqlException(response, sqlEx);
//			}
//			catch (Exception ex)
//			{
//				HandleGeneralException(response, ex);
//			}
//			return response;
//		}

//		public async Task<ServiceResponse<int?>> Post(Tenant tenant)
//		{
//			var response = new ServiceResponse<int?>();
//			try
//			{
//				using (var connection = new SqlConnection(_connectionString))
//				{
//					var sql = "INSERT INTO zb.Tenants (ObjectId, Name, Email, Phone, Address, CompanyName, CompanyEmail, CompanyPhone, CompanyAddress, Website, Products, Status) " +
//							  "VALUES (@ObjectId, @Name, @Email, @Phone, @Address, @CompanyName, @CompanyEmail, @CompanyPhone, @CompanyAddress, @Website, @Products, @Status); " +
//							  "SELECT SCOPE_IDENTITY();";

//					var lastInsertedId = await connection.ExecuteScalarAsync<int?>(sql, tenant);

//					if (lastInsertedId != null)
//					{
//						response.Data = lastInsertedId.Value;
//						response.IsSuccess = true;
//					}
//					else
//					{
//						response.IsSuccess = false;
//						response.ErrorMessage = "Failed to insert Tenant.";
//						_logger.LogError("Failed to insert Tenant.");
//					}
//				}
//			}
//			catch (SqlException sqlEx)
//			{
//				HandleSqlException(response, sqlEx);
//			}
//			catch (Exception ex)
//			{
//				HandleGeneralException(response, ex);
//			}
//			return response;
//		}

//		public async Task<ServiceResponse<bool>> Update(Tenant tenant)
//		{
//			var response = new ServiceResponse<bool>();
//			try
//			{
//				using (var connection = new SqlConnection(_connectionString))
//				{
//					var sql = "UPDATE zb.Tenants SET Name = @Name, Email = @Email, Phone = @Phone, Address = @Address, " +
//							  "CompanyName = @CompanyName, CompanyEmail = @CompanyEmail, CompanyPhone = @CompanyPhone, " +
//							  "CompanyAddress = @CompanyAddress, Website = @Website, Products = @Products, Status = @Status" +
//							  "WHERE ObjectId = @ObjectId";

//					var rowsAffected = await connection.ExecuteAsync(sql, tenant);

//					if (rowsAffected > 0)
//					{
//						response.Data = true;
//						response.IsSuccess = true;
//					}
//					else
//					{
//						response.IsSuccess = false;
//						response.ErrorMessage = "Failed to update Tenant.";
//						_logger.LogError("Failed to update Tenant.");
//					}
//				}
//			}
//			catch (SqlException sqlEx)
//			{
//				HandleSqlException(response, sqlEx);
//			}
//			catch (Exception ex)
//			{
//				HandleGeneralException(response, ex);
//			}
//			return response;
//		}

//		public async Task<ServiceResponse<bool>> Delete(int id)
//		{
//			var response = new ServiceResponse<bool>();
//			try
//			{
//				using (var connection = new SqlConnection(_connectionString))
//				{
//					var sql = "DELETE FROM zb.Tenants WHERE ID = @ID";

//					var rowsAffected = await connection.ExecuteAsync(sql, new { ID = id });

//					if (rowsAffected > 0)
//					{
//						response.Data = true;
//						response.IsSuccess = true;
//					}
//					else
//					{
//						response.IsSuccess = false;
//						response.ErrorMessage = "Failed to delete Tenant.";
//						_logger.LogError("Failed to delete Tenant.");
//					}
//				}
//			}
//			catch (SqlException sqlEx)
//			{
//				HandleSqlException(response, sqlEx);
//			}
//			catch (Exception ex)
//			{
//				HandleGeneralException(response, ex);
//			}
//			return response;
//		}

//		private void HandleSqlException<T>(ServiceResponse<T> response, SqlException sqlEx)
//		{
//			response.IsSuccess = false;
//			_logger.LogError($"SQL Error: {sqlEx.Message}");
//			response.ErrorMessage = $"SQL Error: {sqlEx.Message}";
//		}

//		private void HandleGeneralException<T>(ServiceResponse<T> response, Exception ex)
//		{
//			response.IsSuccess = false;
//			_logger.LogError($"An error occurred: {ex.Message}");
//			response.ErrorMessage = $"An error occurred: {ex.Message}";
//		}
//	}
//}