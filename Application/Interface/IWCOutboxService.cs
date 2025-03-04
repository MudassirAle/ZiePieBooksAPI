using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IWCOutboxService
    {
        Task<ServiceResponse<List<WCOutbox>>> GetAll();
        Task<ServiceResponse<WCOutbox>> GetById(int id);
        Task<ServiceResponse<int?>> Post(WCOutbox wCOutbox);
        Task<ServiceResponse<bool>> Update(WCOutbox wCOutbox);
        Task<ServiceResponse<bool>> Delete(int id);
    }
}
