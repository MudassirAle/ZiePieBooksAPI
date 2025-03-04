using Core.Model;
using Core.Model.Plaid;

namespace Application.Interface
{
    public interface IPlaidInstitutionService
    {
        Task<ServiceResponse<PlaidInstitution>> GetByInstitutionId(string institutionId);
        //Task<ServiceResponse<int?>> Post(PlaidInstitution plaidInstitution);
        Task<ServiceResponse<bool>> PostBulk(List<PlaidInstitution> plaidInstitutions);
    }
}