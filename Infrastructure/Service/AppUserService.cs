using Application.Interface;
using Core.Model;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Data.SqlClient;

namespace Infrastructure.Service
{
	public class AppUserService : IAppUserService
	{
		private readonly GraphServiceClient _graphServiceClient;
		private readonly string _connectionString;
		private readonly ILogger<AppUserService> _logger;

		public AppUserService(
			IConfiguration configuration,
			ILogger<AppUserService> logger,
			GraphServiceClient graphServiceClient
		)
		{
			_connectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
			_logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
			_graphServiceClient = graphServiceClient ?? throw new ArgumentNullException(nameof(graphServiceClient), "Graph service client is null");
		}

		public async Task<ServiceResponse<AppUser>> GetByObjectID(string objectId)
		{
			var response = new ServiceResponse<AppUser>();
			try
			{
				using (var connection = new SqlConnection(_connectionString))
				{
					var appUser = await connection.QueryFirstOrDefaultAsync<AppUser>("SELECT * FROM zb.AppUsers WHERE ObjectId = @ObjectId AND IsActive = 1", new { ObjectId = objectId });

					if (appUser != null)
					{
						response.Data = appUser;
						response.IsSuccess = true;
					}
					else
					{
						response.IsSuccess = false;
						response.ErrorMessage = "AppUser not found.";
					}
				}
			}
			catch (SqlException sqlEx)
			{
				HandleSqlException(response, sqlEx);
			}
			catch (Exception ex)
			{
				HandleGeneralException(response, ex);
			}
			return response;
		}

		public async Task<ServiceResponse<Microsoft.Graph.Models.User>> PostB2CUser(AppUser appUser)
		{
			var response = new ServiceResponse<Microsoft.Graph.Models.User>();
			var password = RandomPassword();
			try
			{
				var requestBody = new Microsoft.Graph.Models.User
				{
					DisplayName = appUser.Name,
					MailNickname = appUser.Name.Replace(" ", "."),
					GivenName = appUser.Name,

					Identities = new List<ObjectIdentity>
					{
						new ObjectIdentity
						{
							SignInType = "emailAddress",
							Issuer = "ziepiebooks.onmicrosoft.com",
							IssuerAssignedId = appUser.Email
						},
					},
					PasswordProfile = new PasswordProfile
					{
						Password = password,
						ForceChangePasswordNextSignIn = true,
					},
				};

				var result = await _graphServiceClient.Users.PostAsync(requestBody);

				//await _graphServiceClient.Users[result!.Id].DeleteAsync();

				result!.PasswordProfile = new PasswordProfile { Password = password };

				response.IsSuccess = true;
				response.Data = result;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				_logger.LogError($"An error occurred: {ex.Message}");
				response.ErrorMessage = $"An error occurred while creating AppUser in B2C Directory: {ex.Message}";
			}
			return response;
		}

		public async Task<ServiceResponse<bool>> DeleteB2CUser(string objectId)
		{
			var response = new ServiceResponse<bool>();

			try
			{
				// Construct the request to delete the user
				await _graphServiceClient.Users[objectId].DeleteAsync();

				response.IsSuccess = true;
				response.Data = true; // Indicates successful deletion
			}
			catch (ServiceException ex)
			{
				response.IsSuccess = false;
				_logger.LogError($"An error occurred: {ex.Message}");
				response.ErrorMessage = $"An error occurred while deleting user with ObjectId {objectId}: {ex.Message}";
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				_logger.LogError($"An unexpected error occurred: {ex.Message}");
				response.ErrorMessage = $"An unexpected error occurred while deleting user with ObjectId {objectId}: {ex.Message}";
			}

			return response;
		}

        public async Task<ServiceResponse<bool>> RevokeUserSessions(string objectId)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                // Call the Graph API to revoke the user's sessions
                var result = await _graphServiceClient.Users[objectId].RevokeSignInSessions
                    .PostAsRevokeSignInSessionsPostResponseAsync();

                // Check the result or response details if needed
                if (result!.Value == true)
                {
                    response.IsSuccess = true;
                    response.Data = true; // Indicates successful session revocation
                }
                else
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = "Failed to revoke sessions, result was null.";
                }
            }
            catch (ServiceException ex)
            {
                response.IsSuccess = false;
                _logger.LogError($"An error occurred while revoking sessions: {ex.Message} - {ex.RawResponseBody}");
                response.ErrorMessage = $"An error occurred while revoking sessions for user with ID {objectId}: {ex.Message}";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                _logger.LogError($"An unexpected error occurred: {ex.Message}");
                response.ErrorMessage = $"An unexpected error occurred while revoking sessions for user with ID {objectId}: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<int?>> Post(AppUser appUser)
		{
			var response = new ServiceResponse<int?>();
			try
			{
				using (var connection = new SqlConnection(_connectionString))
				{
					var sql = "INSERT INTO zb.AppUsers (ObjectId, Name, Email, Phone, Role) " +
							  "VALUES (@ObjectId, @Name, @Email, @Phone, @Role); " +
							  "SELECT SCOPE_IDENTITY();";

					var lastInsertedId = await connection.ExecuteScalarAsync<int?>(sql, appUser);

					if (lastInsertedId != null)
					{
						response.Data = lastInsertedId.Value;
						response.IsSuccess = true;
					}
					else
					{
						response.IsSuccess = false;
						response.ErrorMessage = "Failed to insert AppUser.";
						_logger.LogError("Failed to insert AppUser.");
					}
				}
			}
			catch (SqlException sqlEx)
			{
				HandleSqlException(response, sqlEx);
			}
			catch (Exception ex)
			{
				HandleGeneralException(response, ex);
			}
			return response;
		}

        public async Task<ServiceResponse<bool>> Delete(string objectId)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var rowsAffected = await connection.ExecuteAsync("UPDATE zb.AppUsers SET IsActive = 0 WHERE ObjectId = @ObjectId", new { ObjectId = objectId });

                    if (rowsAffected > 0)
                    {
                        response.IsSuccess = true;
                        response.Data = true;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "AppUser not found.";
                        _logger.LogWarning("AppUser not found for deletion.");
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                HandleSqlException(response, sqlEx);
            }
            catch (Exception ex)
            {
                HandleGeneralException(response, ex);
            }

            return response;
        }

        private void HandleSqlException<T>(ServiceResponse<T> response, SqlException sqlEx)
		{
			response.IsSuccess = false;
			_logger.LogError($"SQL Error: {sqlEx.Message}");
			response.ErrorMessage = $"SQL Error: {sqlEx.Message}";
		}

		private void HandleGeneralException<T>(ServiceResponse<T> response, Exception ex)
		{
			response.IsSuccess = false;
			_logger.LogError($"An error occurred: {ex.Message}");
			response.ErrorMessage = $"An error occurred: {ex.Message}";
		}

		private static string RandomPassword()
		{
			var random = new Random();
			var password = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 2).Select(s => s[random.Next(s.Length)]).ToArray());
			password += new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz", 2).Select(s => s[random.Next(s.Length)]).ToArray());
			password += new string(Enumerable.Repeat("0123456789", 2).Select(s => s[random.Next(s.Length)]).ToArray());
			password += new string(Enumerable.Repeat("!@#$%^&*()_+", 2).Select(s => s[random.Next(s.Length)]).ToArray());
			return password;
		}
	}
}