using Core.Model;

namespace ZiePieBooksAPI.Helper
{
    public static class ResponseHelper
    {
        public static ServiceResponse<T> CreateSuccessResponse<T>(T data)
        {
            return new ServiceResponse<T>
            {
                Data = data,
                IsSuccess = true,
                ErrorMessage = null
            };
        }

        public static ServiceResponse<T> CreateErrorResponse<T>(string errorMessage)
        {
            return new ServiceResponse<T>
            {
                Data = default, // Set default value for generic type T
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }
}
