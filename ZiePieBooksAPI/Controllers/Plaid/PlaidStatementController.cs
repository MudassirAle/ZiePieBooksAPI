using Application.Interface;
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
    public class PlaidStatementController : ControllerBase
    {
        private readonly IPlaidStatementService plaidStatementService;
        private readonly IAppUserService appUserService;
        private readonly ILogger<PlaidStatementController> logger;

        public PlaidStatementController(IPlaidStatementService plaidStatementService, IAppUserService appUserService, ILogger<PlaidStatementController> logger)
        {
            this.plaidStatementService = plaidStatementService ?? throw new ArgumentNullException(nameof(plaidStatementService), "Plaid statement service is null");
            this.appUserService = appUserService ?? throw new ArgumentNullException(nameof(appUserService), "App user service is null");
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        [HttpGet("statementId/{statementId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetByStatementId(string statementId)
        {
            try
            {
                var response = await plaidStatementService.GetByStatementId(statementId);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve Plaid Statement with StatementId {statementId}: {response.ErrorMessage}");
                    return NotFound(response);
                }

                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching Plaid Statement with StatementId {statementId}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpGet("accountId/{accountId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetByAccountId(int accountId)
        {
            try
            {
                var response = await plaidStatementService.GetByAccountId(accountId);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve Plaid Statement with AccountId {accountId}: {response.ErrorMessage}");
                    return NotFound(response);
                }

                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching Plaid Statement with AccountId {accountId}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPost]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Post([FromBody] PlaidStatement plaidStatement)
        {
            if (plaidStatement == null)
            {
                logger.LogWarning("Plaid Statement creation request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            try
            {
                var dbResponse = await plaidStatementService.Post(plaidStatement);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to post Plaid Statement in Database: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to post plaid statement in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse("All plaid statements have been successfully posted."));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while creating new Plaid Statement: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPut]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Update([FromBody] PlaidStatement plaidStatement)
        {
            if (plaidStatement == null)
            {
                logger.LogWarning("Plaid Statement update request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            try
            {
                var dbResponse = await plaidStatementService.Update(plaidStatement);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to update Plaid Statement in Database: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to update plaid statement in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while updating Plaid Statement: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var dbResponse = await plaidStatementService.Delete(id);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to delete Plaid Statement with ID {id}: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to delete plaid statement in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while deleting Plaid Statement with ID {id}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }
    }
}