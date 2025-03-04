using Application.Interface;
using ZiePieBooksAPI.Helper;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITokenService _tokenService;
    private readonly ILogger<TokenValidationMiddleware> _logger;

    public TokenValidationMiddleware(RequestDelegate next, ITokenService tokenService, ILogger<TokenValidationMiddleware> logger)
    {
        _next = next;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (!string.IsNullOrEmpty(authorizationHeader))
        {
            var token = authorizationHeader.Split(" ").Last();
            var objectId = TokenHelper.GetObjectIdFromAccessToken(authorizationHeader, _logger);

            if (objectId == "Not Available" || await _tokenService.IsTokenRevoked(objectId, token))
            {
                _logger.LogWarning($"Access denied for revoked token or invalid object ID: {objectId}");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Token is revoked or invalid.");
                return;
            }
        }

        await _next(context);
    }
}