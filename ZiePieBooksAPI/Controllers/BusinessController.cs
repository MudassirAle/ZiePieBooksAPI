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
	public class BusinessController : ControllerBase
	{
		private readonly IBusinessService businessService;
		private readonly ILogger<BusinessController> logger;

		public BusinessController(IBusinessService businessService, ILogger<BusinessController> logger)
		{
			this.businessService = businessService ?? throw new ArgumentNullException(nameof(businessService), "Business service is null");
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
		}

        [HttpGet("hierarchy/{businessId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetHierarchy(int businessId)
        {
            try
            {
                // Call the GetHierarchy method from the QBFileSyncService
                var hierarchyResponse = await businessService.GetHierarchy(businessId);

                // Check if the service call was successful
                if (!hierarchyResponse.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve hierarchy for Business ID {businessId}: {hierarchyResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to retrieve hierarchy."));
                }

                // Return the hierarchy data
                return Ok(ResponseHelper.CreateSuccessResponse(hierarchyResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching hierarchy for Business ID {businessId}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpGet]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var response = await businessService.GetAll();
				if (!response.IsSuccess)
				{
					logger.LogError($"Failed to retrieve all Businesses: {response.ErrorMessage}");
					return NotFound(response);
				}
				return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while fetching all Businesses: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

		[HttpGet("{id}")]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
		public async Task<IActionResult> GetById(int id)
		{
			try
			{
				var response = await businessService.GetById(id);
				if (!response.IsSuccess)
				{
					logger.LogError($"Failed to retrieve Business with ID {id}: {response.ErrorMessage}");
					return NotFound(response);
				}

				return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while fetching Business with ID {id}: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

        [HttpGet("tenantId/{tenantId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetByTenantId(int tenantId)
        {
            try
            {
                var response = await businessService.GetByTenantId(tenantId);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve Businesses with TenantId {tenantId}: {response.ErrorMessage}");
                    return NotFound(response);
                }

                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching Businesses with TenantId {tenantId}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpGet("customerId/{customerId}")]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
		public async Task<IActionResult> GetByCustomerId(int customerId)
		{
			try
			{
				var response = await businessService.GetByCustomerId(customerId);
				if (!response.IsSuccess)
				{
					logger.LogError($"Failed to retrieve Businesses with CustomerId {customerId}: {response.ErrorMessage}");
					return NotFound(response);
				}

				return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while fetching Businesses with CustomerId {customerId}: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

        [HttpPost]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
		public async Task<IActionResult> Post([FromBody] Business business)
		{
			if (business == null)
			{
				logger.LogWarning("Business creation request body is null.");
				return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
			}

			try
			{
				var dbResponse = await businessService.Post(business);
				if (!dbResponse.IsSuccess)
				{
					logger.LogError($"Failed to post Business in Database: {dbResponse.ErrorMessage}");
					return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to post business in Database."));
				}

				return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while creating new Business: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

		[HttpPut]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
		public async Task<IActionResult> Update([FromBody] Business business)
		{
			if (business == null)
			{
				logger.LogWarning("Business update request body is null.");
				return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
			}

			try
			{
				var dbResponse = await businessService.Update(business);
				if (!dbResponse.IsSuccess)
				{
					logger.LogError($"Failed to update Business in Database: {dbResponse.ErrorMessage}");
					return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to update business in Database."));
				}

				return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while updating Business: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

		[HttpDelete("{id}")]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var dbResponse = await businessService.Delete(id);
				if (!dbResponse.IsSuccess)
				{
					logger.LogError($"Failed to delete Business with ID {id}: {dbResponse.ErrorMessage}");
					return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to delete business in Database."));
				}

				return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while deleting Business with ID {id}: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}
	}
}
