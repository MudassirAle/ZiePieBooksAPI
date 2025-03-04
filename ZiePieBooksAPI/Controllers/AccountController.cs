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
	public class AccountController : ControllerBase
	{
		private readonly IAccountService accountService;
		private readonly ILogger<AccountController> logger;

		public AccountController(IAccountService accountService, ILogger<AccountController> logger)
		{
			this.accountService = accountService ?? throw new ArgumentNullException(nameof(accountService), "Account service is null");
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
		}

		[HttpGet]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var response = await accountService.GetAll();
				if (!response.IsSuccess)
				{
					logger.LogError($"Failed to retrieve all Accounts: {response.ErrorMessage}");
					return NotFound(response);
				}
				return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while fetching all Accounts: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

		[HttpGet("{id}")]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
		public async Task<IActionResult> GetById(int id)
		{
			try
			{
				var response = await accountService.GetById(id);
				if (!response.IsSuccess)
				{
					logger.LogError($"Failed to retrieve Account with ID {id}: {response.ErrorMessage}");
					return NotFound(response);
				}

				return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while fetching Account with ID {id}: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

		[HttpGet("businessId/{businessId}")]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
		public async Task<IActionResult> GetByBusinessId(int businessId)
		{
			try
			{
				var response = await accountService.GetByBusinessId(businessId);
				if (!response.IsSuccess)
				{
					logger.LogError($"Failed to retrieve Account with BusinessId {businessId}: {response.ErrorMessage}");
					return NotFound(response);
				}

				return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while fetching Account with BusinessId {businessId}: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

		[HttpPost]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
		public async Task<IActionResult> Post([FromBody] Account account)
		{
			if (account == null)
			{
				logger.LogWarning("Account creation request body is null.");
				return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
			}

			try
			{
				var dbResponse = await accountService.Post(account);
				if (!dbResponse.IsSuccess)
				{
					logger.LogError($"Failed to post Account in Database: {dbResponse.ErrorMessage}");
					return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to post account in Database."));
				}

				return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while creating new Account: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

		[HttpPut]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
		public async Task<IActionResult> Update([FromBody] Account account)
		{
			if (account == null)
			{
				logger.LogWarning("Account update request body is null.");
				return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
			}

			try
			{
				var dbResponse = await accountService.Update(account);
				if (!dbResponse.IsSuccess)
				{
					logger.LogError($"Failed to update Account in Database: {dbResponse.ErrorMessage}");
					return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to update account in Database."));
				}

				return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while updating Account: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

		[HttpDelete("{id}")]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var dbResponse = await accountService.Delete(id);
				if (!dbResponse.IsSuccess)
				{
					logger.LogError($"Failed to delete Account with ID {id}: {dbResponse.ErrorMessage}");
					return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to delete account in Database."));
				}

				return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while deleting Account with ID {id}: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}
	}
}