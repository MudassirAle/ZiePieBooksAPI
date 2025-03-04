using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface ISubAdminService
    {
        Task<ServiceResponse<List<SubAdmin>>> GetAll();
        Task<ServiceResponse<SubAdmin>> GetByID(int id);
        Task<ServiceResponse<SubAdmin>> GetByObjectID(string objectId);
        Task<ServiceResponse<List<SubAdmin>>> GetByAdminId(int tenantId);
        Task<ServiceResponse<int?>> Post(SubAdmin subAdmin);
        Task<ServiceResponse<bool>> Update(SubAdmin subAdmin);
        Task<ServiceResponse<bool>> UpdateObjectId(string email, string objectId);
        Task<ServiceResponse<bool>> Delete(int id);
    }
}
