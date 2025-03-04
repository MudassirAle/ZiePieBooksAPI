using Core.Model;

namespace Application.Interface
{
    public interface IQBManualAccountService
    {
        Task<ServiceResponse<List<QBManualAccountResponse>>> GetByBusinessId(int businessId);
        Task<ServiceResponse<int?>> Post(QBManualAccount qBManualAccount);
    }
}
