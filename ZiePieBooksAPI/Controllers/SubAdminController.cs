using Application.Interface;
using Core.Model;
using Infrastructure.Service;
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
    public class SubAdminController : ControllerBase
    {
        private readonly ISubAdminService subAdminService;
        private readonly ILogger<SubAdminController> logger;

        public SubAdminController(ISubAdminService subAdminService, ILogger<SubAdminController> logger)
        {
            this.subAdminService = subAdminService ?? throw new ArgumentNullException(nameof(subAdminService), "SubAdmin service is null");
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        [HttpGet]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await subAdminService.GetAll();
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve all SubAdmins: {response.ErrorMessage}");
                    return NotFound(response);
                }
                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching all SubAdmins: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpGet("{id}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetByID(int id)
        {
            try
            {
                var response = await subAdminService.GetByID(id);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve SubAdmin with ID {id}: {response.ErrorMessage}");
                    return NotFound(response);
                }
                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching SubAdmin with ID {id}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpGet("adminId/{adminId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetByAdminId(int adminId)
        {
            try
            {
                var response = await subAdminService.GetByAdminId(adminId);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve SubAdmins for AdminId '{adminId}': {response.ErrorMessage}");
                    return NotFound(response);
                }
                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching SubAdmins for AdminId '{adminId}': {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
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
                var response = await subAdminService.GetByObjectID(objectId);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve SubAdmin with ObjectId {objectId}: {response.ErrorMessage}");
                    return NotFound(response);
                }
                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching SubAdmin with ObjectId {objectId}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPost]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Post([FromBody] SubAdmin subAdmin)
        {
            if (subAdmin == null)
            {
                logger.LogWarning("SubAdmin creation request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            try
            {
                var dbResponse = await subAdminService.Post(subAdmin);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to post SubAdmin in Database: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to post subAdmin in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while creating new SubAdmin: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPut]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Update([FromBody] SubAdmin subAdmin)
        {
            if (subAdmin == null)
            {
                logger.LogWarning("SubAdmin update request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            try
            {
                var dbResponse = await subAdminService.Update(subAdmin);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to update SubAdmin in Database: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to update subAdmin in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while updating SubAdmin: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var dbResponse = await subAdminService.Delete(id);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to delete SubAdmin with ID {id}: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to delete subAdmin in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while deleting SubAdmin with ID {id}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }
    }
}