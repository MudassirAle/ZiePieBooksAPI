using Core.Model;

namespace Application.Interface
{
    public interface IAccountService
    {
        Task<ServiceResponse<List<Account>>> GetAll();
        Task<ServiceResponse<Account>> GetById(int id);
        Task<ServiceResponse<List<Account>>> GetByBusinessId(int businessId);
        Task<ServiceResponse<int?>> Post(Account account);
        Task<ServiceResponse<bool>> Update(Account account);
        Task<ServiceResponse<bool>> Delete(int id);
    }
}
