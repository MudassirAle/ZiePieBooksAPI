using Core.Model;
using Core.Model.Plaid;

namespace Application.Interface
{
    public interface IPlaidTransactionService
    {
        Task<ServiceResponse<PlaidTransaction>> GetByAccountId(string accountId);
        Task<ServiceResponse<int?>> Post(PlaidTransaction plaidTransaction);
        Task<ServiceResponse<bool>> Update(PlaidTransaction plaidTransaction);
        Task<ServiceResponse<bool>> Delete(int id);
    }
}
