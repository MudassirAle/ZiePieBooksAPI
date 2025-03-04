using Core.Model;
using Core.Model.QBDesktop;

namespace Application.Interface.QBDesktop
{
    public interface IQBContactService
    {
        Task<ServiceResponse<List<QBContact>>> GetByTicket(string ticket);
    }
}
