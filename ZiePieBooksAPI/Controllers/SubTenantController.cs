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
    public class SubTenantController : ControllerBase
    {
        private readonly ISubTenantService subTenantService;
        private readonly ILogger<SubTenantController> logger;

        public SubTenantController(ISubTenantService subTenantService, ILogger<SubTenantController> logger)
        {
            this.subTenantService = subTenantService ?? throw new ArgumentNullException(nameof(subTenantService), "SubTenant service is null");
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        [HttpGet]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await subTenantService.GetAll();
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve all SubTenants: {response.ErrorMessage}");
                    return NotFound(response);
                }
                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching all SubTenants: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpGet("{id}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetByID(int id)
        {
            try
            {
                var response = await subTenantService.GetByID(id);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve SubTenant with ID {id}: {response.ErrorMessage}");
                    return NotFound(response);
                }
                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching SubTenant with ID {id}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpGet("tenantId/{tenantId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetByTenantId(int tenantId)
        {
            try
            {
                var response = await subTenantService.GetByTenantId(tenantId);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve Sub Tenants for TenantId '{tenantId}': {response.ErrorMessage}");
                    return NotFound(response);
                }
                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching Sub Tenants for TenantId '{tenantId}': {ex.Message}");
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
                var response = await subTenantService.GetByObjectID(objectId);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve SubTenant with ObjectId {objectId}: {response.ErrorMessage}");
                    return NotFound(response);
                }
                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching SubTenant with ObjectId {objectId}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPost]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Post([FromBody] SubTenant subTenant)
        {
            if (subTenant == null)
            {
                logger.LogWarning("SubTenant creation request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            try
            {
                var dbResponse = await subTenantService.Post(subTenant);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to post SubTenant in Database: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to post subTenant in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while creating new SubTenant: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPut]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Update([FromBody] SubTenant subTenant)
        {
            if (subTenant == null)
            {
                logger.LogWarning("SubTenant update request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            try
            {
                var dbResponse = await subTenantService.Update(subTenant);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to update SubTenant in Database: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to update subTenant in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while updating SubTenant: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var dbResponse = await subTenantService.Delete(id);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to delete SubTenant with ID {id}: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to delete subTenant in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while deleting SubTenant with ID {id}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }
    }
}
