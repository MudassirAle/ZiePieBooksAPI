using Application.Interface;
using Core.Model;
using Core.Model.Plaid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace Infrastructure.Service
{
	public class PlaidAccountService : IPlaidAccountService
	{
		private readonly string _connectionString;
		private readonly ILogger<PlaidAccountService> _logger;

		public PlaidAccountService(IConfiguration configuration, ILogger<PlaidAccountService> logger)
		{
			_connectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
			_logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
		}

		public async Task<ServiceResponse<List<PlaidAccount>>> GetAll()
		{
			var response = new ServiceResponse<List<PlaidAccount>>();
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					string sql = "SELECT * FROM zb.PlaidAccount WHERE IsActive = 1 Order By ID desc"; // Adjust SQL query as per your database schema

					using (SqlCommand command = new SqlCommand(sql, connection))
					using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
					{
						List<PlaidAccount> plaidAccounts = new List<PlaidAccount>();

						while (await dataReader.ReadAsync())
						{
							PlaidAccount plaidAccount = new PlaidAccount
							{
								ID = Convert.ToInt32(dataReader["ID"]),
								BusinessId = Convert.ToInt32(dataReader["BusinessId"]),
								ItemId = dataReader["ItemId"].ToString() ?? string.Empty,
								InstitutionId = dataReader["InstitutionId"].ToString() ?? string.Empty,
								AccountId = dataReader["AccountId"].ToString() ?? string.Empty,
								AccessToken = dataReader["AccessToken"].ToString() ?? string.Empty,
								PlaidBankAccount = JsonConvert.DeserializeObject<PlaidBankAccount>(dataReader["PlaidBankAccount"].ToString() ?? string.Empty) ?? new PlaidBankAccount(),
								LinkedAt = Convert.ToDateTime(dataReader["LinkedAt"]),
								LinkedBy = dataReader["LinkedBy"].ToString() ?? string.Empty,
								LinkedById = Convert.ToInt32(dataReader["LinkedById"]),
								ShareWithCustomer = Convert.ToBoolean(dataReader["ShareWithCustomer"]),
								ShareWithTenant = Convert.ToBoolean(dataReader["ShareWithTenant"]),
								Status = dataReader["Status"].ToString() ?? string.Empty
							};
							plaidAccounts.Add(plaidAccount);
						}

						response.Data = plaidAccounts;
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

		public async Task<ServiceResponse<PlaidAccount>> GetById(int id)
		{
			var response = new ServiceResponse<PlaidAccount>();
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					string sql = "SELECT * FROM zb.PlaidAccount WHERE ID = @ID AND IsActive = 1";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@ID", id);

						using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
						{
							if (await dataReader.ReadAsync())
							{
								PlaidAccount plaidAccount = new PlaidAccount
								{
									ID = Convert.ToInt32(dataReader["ID"]),
									BusinessId = Convert.ToInt32(dataReader["BusinessId"]),
									ItemId = dataReader["ItemId"].ToString() ?? string.Empty,
									InstitutionId = dataReader["InstitutionId"].ToString() ?? string.Empty,
									AccountId = dataReader["AccountId"].ToString() ?? string.Empty,
									AccessToken = dataReader["AccessToken"].ToString() ?? string.Empty,
									PlaidBankAccount = JsonConvert.DeserializeObject<PlaidBankAccount>(dataReader["PlaidBankAccount"].ToString() ?? string.Empty) ?? new PlaidBankAccount(),
									LinkedAt = Convert.ToDateTime(dataReader["LinkedAt"]),
									LinkedBy = dataReader["LinkedBy"].ToString() ?? string.Empty,
									LinkedById = Convert.ToInt32(dataReader["LinkedById"]),
									ShareWithCustomer = Convert.ToBoolean(dataReader["ShareWithCustomer"]),
									ShareWithTenant = Convert.ToBoolean(dataReader["ShareWithTenant"]),
									Status = dataReader["Status"].ToString() ?? string.Empty
								};
								response.Data = plaidAccount;
								response.IsSuccess = true;
							}
							else
							{
								response.ErrorMessage = "PlaidAccount not found.";
								_logger.LogError("PlaidAccount not found.");
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

		public async Task<ServiceResponse<List<PlaidAccount>>> GetByBusinessId(int businessId, string linkedBy)
		{
			var response = new ServiceResponse<List<PlaidAccount>>();
			try
			{
                if (linkedBy.Equals("subTenant", StringComparison.OrdinalIgnoreCase))
                {
                    // Treat subTenant as tenant for query purposes
                    linkedBy = "tenant";
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

                    string sql = "SELECT * FROM zb.PlaidAccount WHERE BusinessId = @BusinessId AND IsActive = 1 AND (LinkedBy = @LinkedBy";

                    if (linkedBy.Equals("tenant", StringComparison.OrdinalIgnoreCase))
                    {
                        sql += " OR ShareWithTenant = 1)";
                    }
                    else if (linkedBy.Equals("customer", StringComparison.OrdinalIgnoreCase))
                    {
                        sql += " OR ShareWithCustomer = 1)";
                    }
                    else
                    {
                        sql += ")";
                    }

                    using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@BusinessId", businessId);
						command.Parameters.AddWithValue("@LinkedBy", linkedBy);

						using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
						{
							List<PlaidAccount> plaidAccounts = new List<PlaidAccount>();

							while (await dataReader.ReadAsync())
							{
								PlaidAccount plaidAccount = new PlaidAccount
								{
									ID = Convert.ToInt32(dataReader["ID"]),
									BusinessId = Convert.ToInt32(dataReader["BusinessId"]),
									ItemId = dataReader["ItemId"].ToString() ?? string.Empty,
									InstitutionId = dataReader["InstitutionId"].ToString() ?? string.Empty,
									AccountId = dataReader["AccountId"].ToString() ?? string.Empty,
									AccessToken = dataReader["AccessToken"].ToString() ?? string.Empty,
									PlaidBankAccount = JsonConvert.DeserializeObject<PlaidBankAccount>(dataReader["PlaidBankAccount"].ToString() ?? string.Empty) ?? new PlaidBankAccount(),
									LinkedAt = Convert.ToDateTime(dataReader["LinkedAt"]),
									LinkedBy = dataReader["LinkedBy"].ToString() ?? string.Empty,
									LinkedById = Convert.ToInt32(dataReader["LinkedById"]),
									ShareWithTenant = Convert.ToBoolean(dataReader["ShareWithTenant"]),
									ShareWithCustomer = Convert.ToBoolean(dataReader["ShareWithCustomer"]),
									Status = dataReader["Status"].ToString() ?? string.Empty
								};
								plaidAccounts.Add(plaidAccount);
							}

							response.Data = plaidAccounts;
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

		public async Task<ServiceResponse<int?>> Post(PlaidAccount plaidAccount)
		{
			var response = new ServiceResponse<int?>();
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					string sql = "INSERT INTO zb.PlaidAccount (BusinessId, ItemId, InstitutionId, AccountId, AccessToken, PlaidBankAccount, LinkedAt, LinkedBy, LinkedById, ShareWithTenant, ShareWithCustomer, Status) " +
								 "VALUES (@BusinessId, @ItemId, @InstitutionId, @AccountId, @AccessToken, @PlaidBankAccount, @LinkedAt, @LinkedBy, @LinkedById, @ShareWithTenant, @ShareWithCustomer, @Status); " +
								 "SELECT SCOPE_IDENTITY();";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@BusinessId", plaidAccount.BusinessId);
						command.Parameters.AddWithValue("@ItemId", plaidAccount.ItemId);
						command.Parameters.AddWithValue("@InstitutionId", plaidAccount.InstitutionId);
						command.Parameters.AddWithValue("@AccountId", plaidAccount.AccountId);
						command.Parameters.AddWithValue("@AccessToken", plaidAccount.AccessToken);
						command.Parameters.AddWithValue("@PlaidBankAccount", JsonConvert.SerializeObject(plaidAccount.PlaidBankAccount));
						command.Parameters.AddWithValue("@LinkedAt", plaidAccount.LinkedAt);
						command.Parameters.AddWithValue("@LinkedBy", plaidAccount.LinkedBy);
						command.Parameters.AddWithValue("@LinkedById", plaidAccount.LinkedById);
						command.Parameters.AddWithValue("@ShareWithTenant", plaidAccount.ShareWithTenant);
						command.Parameters.AddWithValue("@ShareWithCustomer", plaidAccount.ShareWithCustomer);
						command.Parameters.AddWithValue("@Status", plaidAccount.Status);

						int? lastInsertedId = Convert.ToInt32(await command.ExecuteScalarAsync());

						if (lastInsertedId != null)
						{
							response.Data = lastInsertedId;
							response.IsSuccess = true;
						}
						else
						{
							response.ErrorMessage = "Failed to insert PlaidAccount.";
							_logger.LogError("Failed to insert PlaidAccount.");
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

        //public async Task<ServiceResponse<bool>> UpdateStatus(string itemId)
        //{
        //	var response = new ServiceResponse<bool>();
        //	try
        //	{
        //		using (SqlConnection connection = new SqlConnection(_connectionString))
        //		{
        //			await connection.OpenAsync();

        //			string sql = "UPDATE zb.PlaidAccount SET Status = 'Ready' WHERE ItemId = @ItemId";

        //			using (SqlCommand command = new SqlCommand(sql, connection))
        //			{
        //				command.Parameters.AddWithValue("@ItemId", itemId);

        //				int rowsAffected = await command.ExecuteNonQueryAsync();

        //				response.Data = rowsAffected > 0;
        //				response.IsSuccess = rowsAffected > 0;

        //				if (!response.IsSuccess)
        //				{
        //					response.ErrorMessage = "No rows affected.";
        //					_logger.LogError("No rows affected for ItemId: {ItemId}", itemId);
        //				}
        //			}
        //		}
        //	}
        //	catch (SqlException sqlEx)
        //	{
        //		response.IsSuccess = false;
        //		_logger.LogError($"SQL Error: {sqlEx.Message}");
        //		response.ErrorMessage = $"SQL Error: {sqlEx.Message}";
        //	}
        //	catch (Exception ex)
        //	{
        //		response.IsSuccess = false;
        //		_logger.LogError($"An error occurred: {ex.Message}");
        //		response.ErrorMessage = $"An error occurred: {ex.Message}";
        //	}
        //	return response;
        //}

        public async Task<ServiceResponse<bool>> UpdateStatus(string itemId)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction())
                    {
                        var sqlCount = "SELECT COUNT(*) FROM zb.PlaidAccount WHERE ItemId = @ItemId";
                        var sqlUpdate = "UPDATE zb.PlaidAccount SET Status = 'Ready' WHERE ItemId = @ItemId";

                        int initialCount;
                        using (var countCommand = new SqlCommand(sqlCount, connection, transaction))
                        {
                            countCommand.Parameters.AddWithValue("@ItemId", itemId);
                            initialCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync() ?? 0);
                        }

                        const int maxRetries = 3;
                        int rowsAffected = 0;

                        for (int attempt = 1; attempt <= maxRetries; attempt++)
                        {
                            using (var updateCommand = new SqlCommand(sqlUpdate, connection, transaction))
                            {
                                updateCommand.Parameters.AddWithValue("@ItemId", itemId);
                                rowsAffected = await updateCommand.ExecuteNonQueryAsync();
                            }

                            if (rowsAffected == initialCount)
                            {
                                response.Data = true;
                                response.IsSuccess = true;
                                transaction.Commit();
                                return response;
                            }

                            _logger.LogWarning($"Status update attempt {attempt} failed. Expected {initialCount} rows, but updated {rowsAffected} rows.");
                        }

                        transaction.Rollback();
                        response.Data = false;
                        response.IsSuccess = false;
                        response.ErrorMessage = $"Failed to update status. Expected {initialCount} rows, but only {rowsAffected} rows were updated after {maxRetries} attempts.";
                        _logger.LogError(response.ErrorMessage);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                response.IsSuccess = false;
                response.ErrorMessage = $"SQL Error: {sqlEx.Message}";
                _logger.LogError(response.ErrorMessage);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = $"An unexpected error occurred: {ex.Message}";
                _logger.LogError(response.ErrorMessage);
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> Update(PlaidAccount plaidAccount)
		{
			var response = new ServiceResponse<bool>();
			try
			{
				using (SqlConnection connection = new SqlConnection(_connectionString))
				{
					await connection.OpenAsync();

					string sql = "UPDATE zb.PlaidAccount SET " +
								 "BusinessId = @BusinessId, " +
								 "ItemId = @ItemId, " +
								 "InstitutionId = @InstitutionId, " +
								 "AccountId = @AccountId, " +
								 "AccessToken = @AccessToken, " +
								 "PlaidBankAccount = @PlaidBankAccount, " +
								 "LinkedAt = @LinkedAt, " +
								 "LinkedBy = @LinkedBy, " +
								 "LinkedById = @LinkedById, " +
								 "ShareWithTenant = @ShareWithTenant, " +
								 "ShareWithCustomer = @ShareWithCustomer, " +
								 "Status = @Status " +
								 "WHERE ID = @ID";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@ID", plaidAccount.ID);
						command.Parameters.AddWithValue("@BusinessId", plaidAccount.BusinessId);
						command.Parameters.AddWithValue("@ItemId", plaidAccount.ItemId);
						command.Parameters.AddWithValue("@InstitutionId", plaidAccount.InstitutionId);
						command.Parameters.AddWithValue("@AccountId", plaidAccount.AccountId);
						command.Parameters.AddWithValue("@AccessToken", plaidAccount.AccessToken);
						command.Parameters.AddWithValue("@PlaidBankAccount", JsonConvert.SerializeObject(plaidAccount.PlaidBankAccount));
						command.Parameters.AddWithValue("@LinkedAt", plaidAccount.LinkedAt);
						command.Parameters.AddWithValue("@LinkedBy", plaidAccount.LinkedBy);
						command.Parameters.AddWithValue("@LinkedById", plaidAccount.LinkedById);
						command.Parameters.AddWithValue("@ShareWithTenant", plaidAccount.ShareWithTenant);
						command.Parameters.AddWithValue("@ShareWithCustomer", plaidAccount.ShareWithCustomer);
						command.Parameters.AddWithValue("@Status", plaidAccount.Status);

						int rowsAffected = await command.ExecuteNonQueryAsync();

						response.Data = rowsAffected > 0;
						response.IsSuccess = rowsAffected > 0;

						if (!response.IsSuccess)
						{
							response.ErrorMessage = "No rows affected.";
							_logger.LogError("No rows affected.");
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

        public async Task<ServiceResponse<bool>> UpdateAccessToken(string itemId, string accessToken)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "UPDATE zb.PlaidAccount SET " +
                                 "AccessToken = @AccessToken " +
                                 "WHERE ItemId = @ItemId";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ItemId", itemId);
                        command.Parameters.AddWithValue("@AccessToken", accessToken);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        response.Data = rowsAffected > 0;
                        response.IsSuccess = rowsAffected > 0;

                        if (!response.IsSuccess)
                        {
                            response.ErrorMessage = "No rows affected.";
                            _logger.LogError("No rows affected.");
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

					string sql = "UPDATE zb.PlaidAccount SET IsActive = 0 WHERE ID = @ID";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@ID", id);

						int rowsAffected = await command.ExecuteNonQueryAsync();

						response.Data = rowsAffected > 0;
						response.IsSuccess = rowsAffected > 0;

						if (!response.IsSuccess)
						{
							response.ErrorMessage = "No rows affected.";
							_logger.LogError("No rows affected.");
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