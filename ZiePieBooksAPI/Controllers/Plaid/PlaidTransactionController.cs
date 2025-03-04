using Application.Interface;
using Core.Model.Plaid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Text.Json;
using System.Transactions;
using ZiePieBooksAPI.Helper;

namespace AplosMigrationAPI.Controllers
{
    [Authorize]
    [EnableCors("ZiePieBooksCorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class PlaidTransactionController : ControllerBase
    {
        private readonly IPlaidTransactionService plaidTransactionService;
        private readonly IAppUserService appUserService;
        private readonly ILogger<PlaidTransactionController> logger;

        public PlaidTransactionController(IPlaidTransactionService plaidTransactionService, IAppUserService appUserService, ILogger<PlaidTransactionController> logger)
        {
            this.plaidTransactionService = plaidTransactionService ?? throw new ArgumentNullException(nameof(plaidTransactionService), "Plaid transaction service is null");
            this.appUserService = appUserService ?? throw new ArgumentNullException(nameof(appUserService), "App user service is null");
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        //[HttpGet]
        //[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        //public async Task<IActionResult> GetAll()
        //{
        //    try
        //    {
        //        var response = await plaidTransactionService.GetAll();
        //        if (!response.IsSuccess)
        //        {
        //            logger.LogError($"Failed to retrieve all Plaid Transactions: {response.ErrorMessage}");
        //            return NotFound(response);
        //        }
        //        return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError($"An error occurred while fetching all Plaid Transactions: {ex.Message}");
        //        return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
        //    }
        //}

        //[HttpGet("{id}")]
        //[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    try
        //    {
        //        var response = await plaidTransactionService.GetById(id);
        //        if (!response.IsSuccess)
        //        {
        //            logger.LogError($"Failed to retrieve Plaid Transaction with ID {id}: {response.ErrorMessage}");
        //            return NotFound(response);
        //        }

        //        return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError($"An error occurred while fetching Plaid Transaction with ID {id}: {ex.Message}");
        //        return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
        //    }
        //}

        [HttpGet("accountId/{accountId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetByAccountId(string accountId)
        {
            try
            {
                var response = await plaidTransactionService.GetByAccountId(accountId);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve Plaid Transaction with AccountId {accountId}: {response.ErrorMessage}");
                    return NotFound(response);
                }

                //SaveResponseDataToJson(response.Data!);

                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching Plaid Transaction with AccountId {accountId}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPost]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Post([FromBody] PlaidTransaction plaidTransaction)
        {
            if (plaidTransaction == null)
            {
                logger.LogWarning("Plaid Transaction creation request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            try
            {
                plaidTransaction.TotalTransactions = plaidTransaction.Transactions.Count;
                var dbResponse = await plaidTransactionService.Post(plaidTransaction);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to post Plaid Transaction in Database: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to post plaid transaction in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while creating new Plaid Transaction: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPut]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Update([FromBody] PlaidTransaction plaidTransaction)
        {
            if (plaidTransaction == null)
            {
                logger.LogWarning("Plaid Transaction update request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            try
            {
                var existingTransactionResponse = await plaidTransactionService.GetByAccountId(plaidTransaction.AccountId);
                if (!existingTransactionResponse.IsSuccess || existingTransactionResponse.Data == null)
                {
                    logger.LogError($"Plaid Transaction with ID {plaidTransaction.ID} not found: {existingTransactionResponse.ErrorMessage}");
                    return NotFound(ResponseHelper.CreateErrorResponse<object>("Plaid Transaction not found."));
                }

                var existingTransactions = existingTransactionResponse.Data.Transactions ?? new();
                var newTransactions = plaidTransaction.Transactions ?? new();

                var updatedTransactions = existingTransactions
                    .Concat(newTransactions.Where(nt => !existingTransactions.Any(et => et.TransactionId == nt.TransactionId)))
                    .ToList();

                plaidTransaction.TotalTransactions = updatedTransactions.Count;
                plaidTransaction.Transactions = updatedTransactions;

                var dbResponse = await plaidTransactionService.Update(plaidTransaction);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to update Plaid Transaction in Database: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to update plaid transaction in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while updating Plaid Transaction: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var dbResponse = await plaidTransactionService.Delete(id);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to delete Plaid Transaction with ID {id}: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to delete plaid transaction in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while deleting Plaid Transaction with ID {id}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        //private void SaveResponseDataToJson(PlaidTransaction data)
        //{
        //    try
        //    {
        //        // Define the file path and serialize data to JSON
        //        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "responseData.json");
        //        var jsonData = System.Text.Json.JsonSerializer.Serialize(data.Transactions, new JsonSerializerOptions { WriteIndented = true });

        //        // Use System.IO.File to avoid conflict with ControllerBase.File
        //        System.IO.File.WriteAllText(filePath, jsonData);
        //        logger.LogInformation($"Response data saved successfully to {filePath}");
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError($"Failed to save response data to JSON file: {ex.Message}");
        //    }
        //}
    }
}