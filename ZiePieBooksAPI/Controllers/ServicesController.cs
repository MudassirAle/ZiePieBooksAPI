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
    public class ServicesController : ControllerBase
    {
        private readonly IServicesService servicesService;
        private readonly ILogger<ServicesController> logger;

        public ServicesController(IServicesService servicesService, ILogger<ServicesController> logger)
        {
            this.servicesService = servicesService ?? throw new ArgumentNullException(nameof(servicesService), "Services service is null");
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        [HttpGet]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await servicesService.GetAll();
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve all Services: {response.ErrorMessage}");
                    return NotFound(response);
                }
                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching all Services: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpGet("{id}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetByID(int id)
        {
            try
            {
                var response = await servicesService.GetByID(id);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve Service with ID {id}: {response.ErrorMessage}");
                    return NotFound(response);
                }
                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching Service with ID {id}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPost]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Post([FromBody] Services service)
        {
            if (service == null)
            {
                logger.LogWarning("Service creation request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            try
            {
                var dbResponse = await servicesService.Post(service);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to post Service in Database: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to post Service in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while creating new Service: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPut]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Update([FromBody] Services service)
        {
            if (service == null)
            {
                logger.LogWarning("Service update request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            try
            {
                var dbResponse = await servicesService.Update(service);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to update Service in Database: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to update Service in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while updating Service: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var dbResponse = await servicesService.Delete(id);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to delete Service with ID {id}: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to delete Service in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while deleting Service with ID {id}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }
    }
}