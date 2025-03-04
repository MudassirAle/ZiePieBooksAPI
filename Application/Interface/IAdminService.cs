using Core.Model;

namespace Application.Interface
{
    public interface IAdminService
    {
        Task<ServiceResponse<List<Admin>>> GetAll();
        Task<ServiceResponse<Admin>> GetByID(int id);
        Task<ServiceResponse<Admin>> GetByObjectID(string objectId);
        Task<ServiceResponse<int?>> Post(Admin admin);
        Task<ServiceResponse<bool>> Update(Admin admin);
        Task<ServiceResponse<bool>> UpdateObjectId(string email, string objectId);
        Task<ServiceResponse<bool>> Delete(int id);
    }
}
