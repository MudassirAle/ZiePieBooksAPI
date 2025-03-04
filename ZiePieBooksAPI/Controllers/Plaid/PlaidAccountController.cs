using Application.Interface;
using Core.Model;
using Core.Model.Plaid;
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
	public class PlaidAccountController : ControllerBase
	{
		private readonly IPlaidAccountService plaidAccountService;
		private readonly IAppUserService appUserService;
		private readonly ILogger<PlaidAccountController> logger;

		public PlaidAccountController(IPlaidAccountService plaidAccountService, IAppUserService appUserService, ILogger<PlaidAccountController> logger)
		{
			this.plaidAccountService = plaidAccountService ?? throw new ArgumentNullException(nameof(plaidAccountService), "Plaid account service is null");
			this.appUserService = appUserService ?? throw new ArgumentNullException(nameof(appUserService), "App user service is null");
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
		}

        //[HttpGet]
        //[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        //public async Task<IActionResult> GetAll()
        //{
        //	try
        //	{
        //		var response = await plaidAccountService.GetAll();
        //		if (!response.IsSuccess)
        //		{
        //			logger.LogError($"Failed to retrieve all Plaid Accounts: {response.ErrorMessage}");
        //			return NotFound(response);
        //		}
        //		return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
        //	}
        //	catch (Exception ex)
        //	{
        //		logger.LogError($"An error occurred while fetching all Plaid Accounts: {ex.Message}");
        //		return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
        //	}
        //}

        //[HttpGet("{id}")]
        //[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //	try
        //	{
        //		var response = await plaidAccountService.GetById(id);
        //		if (!response.IsSuccess)
        //		{
        //			logger.LogError($"Failed to retrieve Plaid Account with ID {id}: {response.ErrorMessage}");
        //			return NotFound(response);
        //		}

        //		return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
        //	}
        //	catch (Exception ex)
        //	{
        //		logger.LogError($"An error occurred while fetching Plaid Account with ID {id}: {ex.Message}");
        //		return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
        //	}
        //}

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook([FromBody] WebhookModel webhookModel)
        {
            logger.LogInformation($"Received webhook request: ItemId={webhookModel.item_id}, WebhookType={webhookModel.webhook_type}, WebhookCode={webhookModel.webhook_code}");

            try
            {
                if (!string.IsNullOrEmpty(webhookModel.error))
                {
                    logger.LogError($"Webhook error: {webhookModel.error}");
                    return BadRequest();
                }

                if (webhookModel.webhook_type == "TRANSACTIONS")
                {
                    if (webhookModel.webhook_code == "HISTORICAL_UPDATE")
                    {
                        await System.Threading.Tasks.Task.Delay(TimeSpan.FromMinutes(1));

                        var updateResponse = await plaidAccountService.UpdateStatus(webhookModel.item_id);

                        if (!updateResponse.IsSuccess)
                        {
                            logger.LogError($"Failed to update webhook for ItemId {webhookModel.item_id}: {updateResponse.ErrorMessage}");
                            return BadRequest();
                        }

                        logger.LogInformation($"Successfully updated status to 'Ready' for ItemId: {webhookModel.item_id}");
                        return Ok();
                    }
                }

                //if (webhookModel.webhook_type == "STATEMENTS")
                //{
                //    if (webhookModel.webhook_code == "STATEMENTS_REFRESH_COMPLETE")
                //    {
                //        await Task.Delay(TimeSpan.FromMinutes(1));

                //        var updateResponse = await plaidAccountService.UpdateStatus(webhookModel.item_id);

                //        if (!updateResponse.IsSuccess)
                //        {
                //            logger.LogError($"Failed to update webhook for ItemId {webhookModel.item_id}: {updateResponse.ErrorMessage}");
                //            return BadRequest();
                //        }

                //        logger.LogInformation($"Successfully updated status to 'Ready' for ItemId: {webhookModel.item_id}");
                //        return Ok();
                //    }
                //}

                logger.LogInformation($"Received webhook with ItemId: {webhookModel.item_id}, WebhookType: {webhookModel.webhook_type}, WebhookCode: {webhookModel.webhook_code}");
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while processing the webhook: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpGet("businessId/{businessId}")]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
		public async Task<IActionResult> GetByBusinessId(int businessId)
		{
            var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();

            string objectId = TokenHelper.GetObjectIdFromAccessToken(authorizationHeader ?? string.Empty, logger);
            if (objectId == "Not Available")
            {
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Invalid access token."));
            }

			var appUserResponse = await appUserService.GetByObjectID(objectId);
			if (!appUserResponse.IsSuccess)
			{
				logger.LogError($"Failed to fetch app user with objectId {objectId}: {appUserResponse.ErrorMessage}");
				return NotFound(appUserResponse);
			}
            try
			{
				var response = await plaidAccountService.GetByBusinessId(businessId, appUserResponse.Data!.Role);
				if (!response.IsSuccess)
				{
					logger.LogError($"Failed to retrieve Plaid Account with BusinessId {businessId}: {response.ErrorMessage}");
					return NotFound(response);
				}

				return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while fetching Plaid Account with BusinessId {businessId}: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

		[HttpPost]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Post([FromBody] PlaidAccountDTO plaidAccountDTO)
		{
			if (plaidAccountDTO == null)
			{
				logger.LogWarning("Plaid Account creation request body is null.");
				return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
			}

			try
			{
				foreach (var plaidBankAccount in plaidAccountDTO.PlaidBankAccount)
				{
					var plaidAccount = new PlaidAccount
					{
						BusinessId = plaidAccountDTO.BusinessId,
						ItemId = plaidAccountDTO.ItemId,
						InstitutionId = plaidAccountDTO.InstitutionId,
						AccountId = plaidBankAccount.AccountId!,
						AccessToken = plaidAccountDTO.AccessToken,
						PlaidBankAccount = plaidBankAccount,
						LinkedAt = DateTime.UtcNow,
						LinkedBy = plaidAccountDTO.LinkedBy.ToLower(),
						LinkedById = plaidAccountDTO.LinkedById,
						ShareWithTenant = plaidAccountDTO.ShareWithTenant,
						ShareWithCustomer = plaidAccountDTO.ShareWithCustomer,
						Status = plaidAccountDTO.Status,
					};

					var dbResponse = await plaidAccountService.Post(plaidAccount);
					if (!dbResponse.IsSuccess)
					{
						logger.LogError($"Failed to post Plaid Account in Database: {dbResponse.ErrorMessage}");
						return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to post plaid account in Database."));
					}
				}

				return Ok(ResponseHelper.CreateSuccessResponse("All plaid accounts have been successfully posted."));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while creating new Plaid Account: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

		[HttpPut]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
		public async Task<IActionResult> Update([FromBody] PlaidAccount plaidAccount)
		{
			if (plaidAccount == null)
			{
				logger.LogWarning("Plaid Account update request body is null.");
				return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
			}

			try
			{
				var dbResponse = await plaidAccountService.Update(plaidAccount);
				if (!dbResponse.IsSuccess)
				{
					logger.LogError($"Failed to update Plaid Account in Database: {dbResponse.ErrorMessage}");
					return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to update plaid account in Database."));
				}

				return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while updating Plaid Account: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}

        [HttpPut("updateAccessToken")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> UpdateAccessToken([FromBody] UpdatePlaidAccountDTO updatePlaidAccountDTO)
        {
            if (updatePlaidAccountDTO == null)
            {
                logger.LogWarning("Plaid Account update access token request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            try
            {
                var dbResponse = await plaidAccountService.UpdateAccessToken(updatePlaidAccountDTO.ItemId, updatePlaidAccountDTO.AccessToken);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to update access token for Plaid Account with ItemId {updatePlaidAccountDTO.ItemId}: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to update access token in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while updating access token for Plaid Account with ItemId {updatePlaidAccountDTO.ItemId}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpDelete("{id}")]
		[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var dbResponse = await plaidAccountService.Delete(id);
				if (!dbResponse.IsSuccess)
				{
					logger.LogError($"Failed to delete Plaid Account with ID {id}: {dbResponse.ErrorMessage}");
					return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to delete plaid account in Database."));
				}

				return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
			}
			catch (Exception ex)
			{
				logger.LogError($"An error occurred while deleting Plaid Account with ID {id}: {ex.Message}");
				return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
			}
		}
	}
}
