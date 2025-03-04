using Core.Model;
using Core.Model.QBDesktop;

namespace Application.Interface.QBDesktop
{
    public interface IQBAccountService
    {
        Task<ServiceResponse<List<QBAccount>>> GetByTicket(string ticket);
    }
}
