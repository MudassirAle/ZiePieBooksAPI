using Application.Interface;
using Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using ZiePieBooksAPI.Helper;

namespace AplosMigrationAPI.Controllers
{
	[Authorize]
	[EnableCors("ZiePieBooksCorsPolicy")]
	[Route("api/[controller]")]
	[ApiController]
	public class AppUserController : ControllerBase
	{
		private readonly IAppUserService appUserService;
		private readonly ILogger<AppUserController> logger;
		private readonly IEmailService emailService;
		private readonly ITenantService tenantService;
		private readonly ISubTenantService subTenantService;
		private readonly ICustomerService customerService;
		private readonly IAdminService adminService;
		private readonly ISubAdminService subAdminService;

		public AppUserController(
			IAppUserService appUserService,
			ILogger<AppUserController> logger,
			IEmailService emailService,
			ITenantService tenantService,
			ISubTenantService subTenantService,
			ICustomerService customerService,
			IAdminService adminService,
			ISubAdminService subAdminService
			)
		{
			this.appUserService = appUserService;
			this.logger = logger;
			this.emailService = emailService;
			this.tenantService = tenantService;
			this.subTenantService = subTenantService;
			this.customerService = customerService;
			this.adminService = adminService;
			this.subAdminService = subAdminService;
		}

		[HttpGet("objectId")]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
		public async Task<IActionResult> GetByObjectID()
		{
			var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();

			string objectId = TokenHelper.GetObjectIdFromAccessToken(authorizationHeader ?? string.Empty, logger);
			if (objectId == "Not Available")
			{
				return BadRequest(ResponseHelper.CreateErrorResponse<object>("Invalid access token."));
			}
			try
			{
				var response = await appUserService.GetByObjectID(objectId);
				if (!response.IsSuccess)
				{
					logger.LogError($"Failed to retrieve AppUser with ObjectId {objectId}: {response.ErrorMessage}");
					return NotFound(response);
				}
				return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while fetching AppUser with ObjectId {objectId}: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

		[HttpPost("invite")]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
		public async Task<IActionResult> Invite([FromBody] AppUser appUser)
		{
			if (appUser == null)
			{
				logger.LogWarning("Invite request body is null.");
				return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
			}

			try
			{
				var b2cResponse = await appUserService.PostB2CUser(appUser);
				if (!b2cResponse.IsSuccess)
				{
					logger.LogError($"Failed to create new User in B2C: {b2cResponse.ErrorMessage}");
					return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to create new User in B2C."));
				}

				appUser.ObjectId = b2cResponse.Data!.Id!;
				var dbResponse = await appUserService.Post(appUser);
				if (!dbResponse.IsSuccess)
				{
					logger.LogError($"Failed to post User in Database: { dbResponse.ErrorMessage}");
					return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to post user in Database."));
				}

				var emailResponse = await emailService.SendEmailAsync(appUser, b2cResponse.Data.PasswordProfile!.Password!);
				if (!emailResponse.IsSuccess)
				{
					var deleteResult = await appUserService.DeleteB2CUser(appUser.ObjectId);
					logger.LogError($"Failed to send email to new User: {emailResponse.ErrorMessage}");
					return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("Failed to send email to new User."));
				}

                if (appUser.Role.ToLower() == "admin")
                {
                    var adminUpdateResponse = await adminService.UpdateObjectId(appUser.Email, appUser.ObjectId);
                    if (!adminUpdateResponse.IsSuccess)
                    {
                        var deleteResult = await appUserService.DeleteB2CUser(appUser.ObjectId);
                        logger.LogError($"Failed to update ObjectId for admin: {adminUpdateResponse.ErrorMessage}");
                        return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("Failed to update Admin ObjectId"));
                    }
                }

                if (appUser.Role.ToLower() == "subadmin")
                {
                    var subAdminUpdateResponse = await subAdminService.UpdateObjectId(appUser.Email, appUser.ObjectId);
                    if (!subAdminUpdateResponse.IsSuccess)
                    {
                        var deleteResult = await appUserService.DeleteB2CUser(appUser.ObjectId);
                        logger.LogError($"Failed to update ObjectId for subadmin: {subAdminUpdateResponse.ErrorMessage}");
                        return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("Failed to update SubAdmin ObjectId"));
                    }
                }

                if (appUser.Role.ToLower() == "tenant")
				{
					var tenantUpdateResponse = await tenantService.UpdateObjectId(appUser.Email, appUser.ObjectId);
					if (!tenantUpdateResponse.IsSuccess)
					{
						var deleteResult = await appUserService.DeleteB2CUser(appUser.ObjectId);
						logger.LogError($"Failed to update ObjectId for tenant: {tenantUpdateResponse.ErrorMessage}");
						return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("Failed to update Tenant ObjectId"));
					}
				}

                if (appUser.Role.ToLower() == "subtenant")
                {
                    var subTenantUpdateResponse = await subTenantService.UpdateObjectId(appUser.Email, appUser.ObjectId);
                    if (!subTenantUpdateResponse.IsSuccess)
                    {
                        var deleteResult = await appUserService.DeleteB2CUser(appUser.ObjectId);
                        logger.LogError($"Failed to update ObjectId for sub tenant: {subTenantUpdateResponse.ErrorMessage}");
                        return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("Failed to update SubTenant ObjectId"));
                    }
                }

                else if (appUser.Role.ToLower() == "customer")
				{
					var customerUpdateResponse = await customerService.UpdateObjectId(appUser.Email, appUser.ObjectId);
					if (!customerUpdateResponse.IsSuccess)
					{
						var deleteResult = await appUserService.DeleteB2CUser(appUser.ObjectId);
						logger.LogError($"Failed to update ObjectId for customer: {customerUpdateResponse.ErrorMessage}");
						return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("Failed to update Customer ObjectId"));
					}
				}

				return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while creating new AppUser: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

        [HttpDelete("delete/{objectId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> DeleteUser(string objectId)
        {
            if (string.IsNullOrEmpty(objectId))
            {
                logger.LogWarning("Object ID is null or empty.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Object ID cannot be null or empty."));
            }

            try
            {
                // Delete user from Azure AD B2C
                var deleteB2CUserResponse = await appUserService.DeleteB2CUser(objectId);
                if (!deleteB2CUserResponse.IsSuccess)
                {
                    logger.LogError($"Failed to delete User in B2C: {deleteB2CUserResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to delete user in B2C."));
                }

                // Delete user from the database
                var deleteDbResponse = await appUserService.Delete(objectId);
                if (!deleteDbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to delete User in Database: {deleteDbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to delete user in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse<object>("User deleted successfully."));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while deleting the AppUser: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPost("revokeSessions/{objectId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> RevokeUserSessions(string objectId)
        {
            if (string.IsNullOrEmpty(objectId))
            {
                logger.LogWarning("Object ID is null or empty.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Object ID cannot be null or empty."));
            }

            try
            {
                var revokeResponse = await appUserService.RevokeUserSessions(objectId);
                if (!revokeResponse.IsSuccess)
                {
                    logger.LogError($"Failed to revoke sessions for User with ObjectId {objectId}: {revokeResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to revoke user sessions."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse<object>("User sessions revoked successfully."));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while revoking sessions for User with ObjectId {objectId}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }
    }
}