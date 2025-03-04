namespace Application.Interface
{
    public interface ITokenService
    {
        Task<bool> StoreTokenAsync(string objectId, string token);
        Task<bool> IsTokenRevoked(string objectId, string token);
        Task<bool> RevokeTokenAsync(string objectId);
    }
}