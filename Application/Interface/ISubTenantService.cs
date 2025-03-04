using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface ISubTenantService
    {
        Task<ServiceResponse<List<SubTenant>>> GetAll();
        Task<ServiceResponse<SubTenant>> GetByID(int id);
        Task<ServiceResponse<SubTenant>> GetByObjectID(string objectId);
        Task<ServiceResponse<List<SubTenant>>> GetByTenantId(int tenantId);
        Task<ServiceResponse<int?>> Post(SubTenant subTenant);
        Task<ServiceResponse<bool>> Update(SubTenant subTenant);
        Task<ServiceResponse<bool>> UpdateObjectId(string email, string objectId);
        Task<ServiceResponse<bool>> Delete(int id);
    }
}
