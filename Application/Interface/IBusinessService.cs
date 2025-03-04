using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
	public interface IBusinessService
	{
        Task<ServiceResponse<Hierarchy>> GetHierarchy(int businessId);
        Task<ServiceResponse<List<Business>>> GetAll();
		Task<ServiceResponse<List<Business>>> GetByCustomerId(int customerId);
		Task<ServiceResponse<List<Business>>> GetByTenantId(int tenantId);
		Task<ServiceResponse<Business>> GetById(int id);
		Task<ServiceResponse<int?>> Post(Business business);
		Task<ServiceResponse<bool>> Update(Business business);
		Task<ServiceResponse<bool>> Delete(int id);
	}
}
