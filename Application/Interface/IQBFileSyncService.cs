using Core.Model;

namespace Application.Interface
{
    public interface IQBFileSyncService
    {
        Task<ServiceResponse<int?>> Post(QBFileReceiveLog qBFileReceiveLog);
        Task<ServiceResponse<bool>> UpdateStatus(QBFileSyncUpdate qBFileSyncUpdate);
        Task<ServiceResponse<bool>> Delete(int id);
    }
}
