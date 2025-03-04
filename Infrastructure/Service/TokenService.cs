using Application.Interface;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace Infrastructure.Service
{
    public class TokenService : ITokenService
    {
        private readonly string _connectionString;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _connectionString = configuration.GetConnectionString("SAPDev") ?? throw new ArgumentNullException(nameof(configuration), "Configuration or SAP connection string is null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        public async Task<bool> StoreTokenAsync(string objectId, string token)
        {
            const string sql = "INSERT INTO UserTokens (ObjectId, Token, IsRevoked) VALUES (@ObjectId, @Token, 0)";

            using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteAsync(sql, new { ObjectId = objectId, Token = token });

            return result > 0;
        }

        public async Task<bool> IsTokenRevoked(string objectId, string token)
        {
            const string sql = "SELECT IsRevoked FROM UserTokens WHERE ObjectId = @ObjectId AND Token = @Token";

            using var connection = new SqlConnection(_connectionString);
            var isRevoked = await connection.QuerySingleOrDefaultAsync<bool>(sql, new { ObjectId = objectId, Token = token });

            return isRevoked;
        }

        public async Task<bool> RevokeTokenAsync(string objectId)
        {
            const string sql = "UPDATE UserTokens SET IsRevoked = 1 WHERE ObjectId = @ObjectId";

            using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteAsync(sql, new { ObjectId = objectId });

            return result > 0;
        }
    }
}
