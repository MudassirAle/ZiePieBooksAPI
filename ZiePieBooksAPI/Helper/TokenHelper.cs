using System.IdentityModel.Tokens.Jwt;

namespace ZiePieBooksAPI.Helper
{
    public static class TokenHelper
    {
        public static string GetObjectIdFromAccessToken(string authorizationHeader, ILogger logger)
        {
            if (!string.IsNullOrWhiteSpace(authorizationHeader))
            {
                try
                {
                    var accessToken = authorizationHeader.Split(" ").Last();
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;

                    if (jsonToken != null)
                    {
                        var claims = jsonToken.Claims;
                        var objectIdClaim = claims.FirstOrDefault(c => c.Type == "oid");

                        if (objectIdClaim != null)
                        {
                            //logger.LogInformation($"This is the Object Id: {objectIdClaim.Value}");
                            return objectIdClaim.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error decoding access token: {ex.Message}");
                }
            }
            return "Not Available";
        }
        public static string? GetNameFromAccessToken(string authorizationHeader, ILogger logger)
        {
            if (!string.IsNullOrWhiteSpace(authorizationHeader))
            {
                try
                {
                    var accessToken = authorizationHeader.Split(" ").Last();
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;

                    if (jsonToken != null)
                    {
                        var claims = jsonToken.Claims;
                        var nameClaim = claims.FirstOrDefault(c => c.Type == "name");

                        if (nameClaim != null)
                        {
                            logger.LogInformation($"This is the Name: {nameClaim.Value}");
                            return nameClaim.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error decoding access token: {ex.Message}");
                }
            }
            return null;
        }
    }
}
