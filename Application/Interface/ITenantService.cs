using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
	public interface ITenantService
	{
		Task<ServiceResponse<List<Tenant>>> GetAll();
		Task<ServiceResponse<Tenant>> GetByID(int id);
		Task<ServiceResponse<Tenant>> GetByObjectID(string objectId);
		Task<ServiceResponse<int?>> Post(Tenant tenant);
		Task<ServiceResponse<bool>> Update(Tenant tenant);
		Task<ServiceResponse<bool>> UpdatePaymentMethod(int tenantId, string paymentMethod);
		Task<ServiceResponse<bool>> UpdateTeamMember(int tenantId, string teamMember);
		Task<ServiceResponse<bool>> UpdateObjectId(string email, string objectId);
		Task<ServiceResponse<bool>> Delete(int id);
	}
}
