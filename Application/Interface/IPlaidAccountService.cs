using Core.Model;
using Core.Model.Plaid;

namespace Application.Interface
{
	public interface IPlaidAccountService
	{
		Task<ServiceResponse<List<PlaidAccount>>> GetAll();
		Task<ServiceResponse<PlaidAccount>> GetById(int id);
		Task<ServiceResponse<List<PlaidAccount>>> GetByBusinessId(int businessId, string linkedBy);
		Task<ServiceResponse<int?>> Post(PlaidAccount plaidAccount);
		Task<ServiceResponse<bool>> UpdateStatus(string itemId);
		Task<ServiceResponse<bool>> Update(PlaidAccount plaidAccount);
		Task<ServiceResponse<bool>> UpdateAccessToken(string itemId, string accessToken);
		Task<ServiceResponse<bool>> Delete(int id);
	}
}
