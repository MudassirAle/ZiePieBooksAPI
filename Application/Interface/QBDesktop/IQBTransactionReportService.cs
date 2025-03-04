using Core.Model;
using Core.Model.QBDesktop;

namespace Application.Interface.QBDesktop
{
    public interface IQBTransactionReportService
    {
        Task<ServiceResponse<List<QBTransactionReport>>> GetByTicket(string ticket, string startDate, string endDate);
    }
}
