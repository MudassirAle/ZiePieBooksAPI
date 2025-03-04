using Core.Model;

namespace Application.Interface
{
    public interface ITaskService
    {
        Task<ServiceResponse<Core.Model.Task>> GetByOnboardingId(int onboardingId);
        Task<ServiceResponse<int?>> ValidatePassword(ValidatePasswordDTO validatePasswordDTO);
        Task<ServiceResponse<bool>> EnqueueTicket(string ticket);
        Task<ServiceResponse<bool>> UpdateStatus(int onboardingId);
    }
}