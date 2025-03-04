using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
	public interface IAppUserService
	{
		Task<ServiceResponse<AppUser>> GetByObjectID(string objectId);
		Task<ServiceResponse<Microsoft.Graph.Models.User>> PostB2CUser(AppUser appUser);
		Task<ServiceResponse<bool>> DeleteB2CUser(string objectId);
		Task<ServiceResponse<bool>> RevokeUserSessions(string objectId);
		Task<ServiceResponse<int?>> Post(AppUser appUser);
        Task<ServiceResponse<bool>> Delete(string objectId);
    }
}
