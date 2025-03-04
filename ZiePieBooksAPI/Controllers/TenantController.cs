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
	public class TenantController : ControllerBase
	{
		private readonly ITenantService tenantService;
		private readonly ILogger<TenantController> logger;

		public TenantController(ITenantService tenantService, ILogger<TenantController> logger)
		{
			this.tenantService = tenantService ?? throw new ArgumentNullException(nameof(tenantService), "Tenant service is null");
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
		}

		[HttpGet]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var response = await tenantService.GetAll();
				if (!response.IsSuccess)
				{
					logger.LogError($"Failed to retrieve all Tenants: {response.ErrorMessage}");
					return NotFound(response);
				}
				return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while fetching all Tenants: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

		[HttpGet("{id}")]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
		public async Task<IActionResult> GetByID(int id)
		{
			try
			{
				var response = await tenantService.GetByID(id);
				if (!response.IsSuccess)
				{
					logger.LogError($"Failed to retrieve Tenant with ID {id}: {response.ErrorMessage}");
					return NotFound(response);
				}
				return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while fetching Tenant with ID {id}: {ex.Message}");
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
				var response = await tenantService.GetByObjectID(objectId);
				if (!response.IsSuccess)
				{
					logger.LogError($"Failed to retrieve Tenant with ObjectId {objectId}: {response.ErrorMessage}");
					return NotFound(response);
				}
				return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while fetching Tenant with ObjectId {objectId}: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

		[HttpPost]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
		public async Task<IActionResult> Post([FromBody] Tenant tenant)
		{
			if (tenant == null)
			{
				logger.LogWarning("Tenant creation request body is null.");
				return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
			}

			try
			{
				var dbResponse = await tenantService.Post(tenant);
				if (!dbResponse.IsSuccess)
				{
					logger.LogError($"Failed to post Tenant in Database: {dbResponse.ErrorMessage}");
					return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to post tenant in Database."));
				}

				return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while creating new Tenant: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

		[HttpPut]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
		public async Task<IActionResult> Update([FromBody] Tenant tenant)
		{
			if (tenant == null)
			{
				logger.LogWarning("Tenant update request body is null.");
				return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
			}

			try
			{
				var dbResponse = await tenantService.Update(tenant);
				if (!dbResponse.IsSuccess)
				{
					logger.LogError($"Failed to update Tenant in Database: {dbResponse.ErrorMessage}");
					return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to update tenant in Database."));
				}

				return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while updating Tenant: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

        [HttpPut("payment-method/{tenantId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> UpdatePaymentMethod(int tenantId, string paymentMethod)
        {
            if (string.IsNullOrEmpty(paymentMethod))
            {
                logger.LogWarning("Payment method cannot be null or empty.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Payment method cannot be null or empty."));
            }

            try
            {
                var dbResponse = await tenantService.UpdatePaymentMethod(tenantId, paymentMethod);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to update Tenant Payment Method in Database: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to update tenant payment method in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while updating Tenant Payment Method: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPut("team-member/{tenantId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> UpdateTeamMember(int tenantId, string teamMember)
        {
            if (string.IsNullOrEmpty(teamMember))
            {
                logger.LogWarning("Team member cannot be null or empty.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Team member cannot be null or empty."));
            }

            try
            {
                var dbResponse = await tenantService.UpdateTeamMember(tenantId, teamMember);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to update Tenant Team Member Selection in Database: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to update tenant team member selection in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while updating Tenant Member Selection: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpDelete("{id}")]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var dbResponse = await tenantService.Delete(id);
				if (!dbResponse.IsSuccess)
				{
					logger.LogError($"Failed to delete Tenant with ID {id}: {dbResponse.ErrorMessage}");
					return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to delete tenant in Database."));
				}

				return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while deleting Tenant with ID {id}: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}
	}
}
