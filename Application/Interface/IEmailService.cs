using Core.Model;

namespace Application.Interface
{
	public interface IEmailService
	{
		Task<ServiceResponse<bool>> SendEmailAsync(AppUser appUser, string password);
        Task<ServiceResponse<bool>> Send(Email email);
    }
}
