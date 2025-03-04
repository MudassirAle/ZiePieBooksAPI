using Core.Model.QBDesktop;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.QBDesktop
{
    public interface IQBTransactionService
    {
        Task<ServiceResponse<List<QBTransaction>>> GetByTicket(string ticket, string startDate, string endDate);
    }
}
