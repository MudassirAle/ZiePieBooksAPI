using Application.Interface;
using Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using ZiePieBooksAPI.Helper;
using System.Text.Json.Serialization;
using Core.Model.Plaid;

namespace AplosMigrationAPI.Controllers
{
    [Authorize]
    [EnableCors("ZiePieBooksCorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class PlaidInstitutionController : ControllerBase
    {
        private readonly IPlaidInstitutionService plaidInstitutionService;
        private readonly ILogger<PlaidInstitutionController> logger;

        public PlaidInstitutionController(IPlaidInstitutionService plaidInstitutionService, ILogger<PlaidInstitutionController> logger)
        {
            this.plaidInstitutionService = plaidInstitutionService ?? throw new ArgumentNullException(nameof(plaidInstitutionService), "Plaid Institution service is null");
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        [HttpGet("{institutionId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetByInstitutionId(string institutionId)
        {
            try
            {
                var response = await plaidInstitutionService.GetByInstitutionId(institutionId);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve Plaid Institution with ID {institutionId}: {response.ErrorMessage}");
                    return NotFound(response);
                }
                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching Plaid Institution with ID {institutionId}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        //[HttpPost]
        //[RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        //public async Task<IActionResult> Post([FromBody] PlaidInstitution plaidInstitution)
        //{
        //    if (plaidInstitution == null)
        //    {
        //        logger.LogWarning("Plaid Institution creation request body is null.");
        //        return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
        //    }

        //    try
        //    {
        //        var dbResponse = await plaidInstitutionService.Post(plaidInstitution);
        //        if (!dbResponse.IsSuccess)
        //        {
        //            logger.LogError($"Failed to post Plaid Institution in Database: {dbResponse.ErrorMessage}");
        //            return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to post plaid institution in Database."));
        //        }

        //        return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError($"An error occurred while creating new Plaid Institution: {ex.Message}");
        //        return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
        //    }
        //}

        [HttpPost]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Post([FromBody] PlaidInstitutionDTO plaidInstitutionDTO)
        {
            if (plaidInstitutionDTO == null || plaidInstitutionDTO.PlaidInstitutions == null || !plaidInstitutionDTO.PlaidInstitutions.Any())
            {
                logger.LogWarning("Plaid Institution creation request body is null or empty.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null or empty."));
            }

            try
            {
                var dbResponse = await plaidInstitutionService.PostBulk(plaidInstitutionDTO.PlaidInstitutions);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to insert Plaid Institutions in Database: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to bulk insert plaid institutions in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while creating new Plaid Institutions: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

    }
}