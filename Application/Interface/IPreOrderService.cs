using Core.Model;

namespace Application.Interface
{
    public interface IPreOrderService
    {
        Task<ServiceResponse<PreOrder>> GetByBusinessId(int businessId);
        Task<ServiceResponse<PreOrder>> GetById(int id);
        Task<ServiceResponse<int?>> Post(PreOrder preOrder);
        Task<ServiceResponse<bool>> UpdatePayment(int preOrderId, PaymentDTO paymentDTO);
        Task<ServiceResponse<bool>> Update(PreOrder preOrder);
        Task<ServiceResponse<bool>> Delete(int id);
    }
}
