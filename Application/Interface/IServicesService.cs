using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IServicesService
    {
        Task<ServiceResponse<List<Services>>> GetAll();
        Task<ServiceResponse<Services>> GetByID(int id);
        Task<ServiceResponse<int?>> Post(Services services);
        Task<ServiceResponse<bool>> Update(Services services);
        Task<ServiceResponse<bool>> Delete(int id);
    }
}
