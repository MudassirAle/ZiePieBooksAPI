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
	public class CustomerController : ControllerBase
	{
		private readonly ICustomerService customerService;
		private readonly ILogger<CustomerController> logger;

		public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
		{
			this.customerService = customerService ?? throw new ArgumentNullException(nameof(customerService), "Customer service is null");
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
		}

		[HttpGet]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var response = await customerService.GetAll();
				if (!response.IsSuccess)
				{
					logger.LogError($"Failed to retrieve all Customers: {response.ErrorMessage}");
					return NotFound(response);
				}
				return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while fetching all Customers: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

		[HttpGet("{id}")]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
		public async Task<IActionResult> GetById(int id)
		{
			try
			{
				var response = await customerService.GetById(id);
				if (!response.IsSuccess)
				{
					logger.LogError($"Failed to retrieve Customer with ID '{id}': {response.ErrorMessage}");
					return NotFound(response);
				}
				return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while fetching Customer with ID '{id}': {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

		[HttpGet("tenantId/{tenantId}")]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
		public async Task<IActionResult> GetByTenantId(int tenantId)
		{
			try
			{
				var response = await customerService.GetByTenantId(tenantId);
				if (!response.IsSuccess)
				{
					logger.LogError($"Failed to retrieve Customers for TenantId '{tenantId}': {response.ErrorMessage}");
					return NotFound(response);
				}
				return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while fetching Customers for TenantId '{tenantId}': {ex.Message}");
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
				var response = await customerService.GetByObjectId(objectId);
				if (!response.IsSuccess)
				{
					logger.LogError($"Failed to retrieve Customer with ObjectId {objectId}: {response.ErrorMessage}");
					return NotFound(response);
				}

				return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while fetching Customer with ObjectId {objectId}: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

		[HttpPost]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
		public async Task<IActionResult> Post([FromBody] Customer customer)
		{
			if (customer == null)
			{
				logger.LogWarning("Customer creation request body is null.");
				return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
			}

			try
			{
				var dbResponse = await customerService.Post(customer);
				if (!dbResponse.IsSuccess)
				{
					logger.LogError($"Failed to post Customer in Database: {dbResponse.ErrorMessage}");
					return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to post customer in Database."));
				}

				return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while creating new Customer: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

		[HttpPut]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
		public async Task<IActionResult> Update([FromBody] Customer customer)
		{
			if (customer == null)
			{
				logger.LogWarning("Customer update request body is null.");
				return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
			}

			try
			{
				var dbResponse = await customerService.Update(customer);
				if (!dbResponse.IsSuccess)
				{
					logger.LogError($"Failed to update Customer in Database: {dbResponse.ErrorMessage}");
					return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to update customer in Database."));
				}

				return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while updating Customer: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

		[HttpDelete("{id}")]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var dbResponse = await customerService.Delete(id);
				if (!dbResponse.IsSuccess)
				{
					logger.LogError($"Failed to delete Customer with ID {id}: {dbResponse.ErrorMessage}");
					return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to delete customer in Database."));
				}

				return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while deleting Customer with ID {id}: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}
	}
}