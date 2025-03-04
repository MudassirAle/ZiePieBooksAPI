using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
	public interface ICustomerService
	{
		Task<ServiceResponse<List<Customer>>> GetAll();
		Task<ServiceResponse<Customer>> GetById(int id);
		Task<ServiceResponse<List<Customer>>> GetByTenantId(int tenantId);
		Task<ServiceResponse<Customer>> GetByObjectId(string objectId);
		Task<ServiceResponse<int?>> Post(Customer customer);
		Task<ServiceResponse<bool>> Update(Customer customer);
		Task<ServiceResponse<bool>> UpdateObjectId(string email, string objectId);
		Task<ServiceResponse<bool>> Delete(int id);
	}
}
