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
    public class StatementConfigurationController : ControllerBase
    {
        private readonly IStatementConfigurationService statementConfigurationService;
        private readonly ILogger<StatementConfigurationController> logger;

        public StatementConfigurationController(IStatementConfigurationService statementConfigurationService, ILogger<StatementConfigurationController> logger)
        {
            this.statementConfigurationService = statementConfigurationService ?? throw new ArgumentNullException(nameof(statementConfigurationService), "StatementConfiguration service is null");
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        [HttpGet("platformAccountId/{platformAccountId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetByPlatformAccountId(int platformAccountId)
        {
            if (platformAccountId <= 0)
            {
                logger.LogWarning("Invalid PlatformAccountId.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Invalid PlatformAccountId."));
            }

            try
            {
                var response = await statementConfigurationService.GetByPlatformAccountId(platformAccountId);

                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve StatementConfiguration for PlatformAccountId {platformAccountId}: {response.ErrorMessage}");
                    return NotFound(ResponseHelper.CreateErrorResponse<object>($"No StatementConfiguration found for PlatformAccountId {platformAccountId}."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching StatementConfiguration for PlatformAccountId {platformAccountId}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPost("statement-configuration")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> PostStatementConfiguration([FromBody] StatementConfigurationDTO statementConfigurationDTO)
        {
            if (statementConfigurationDTO == null)
            {
                logger.LogWarning("StatementConfiguration creation request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            try
            {
                var dbResponse = await statementConfigurationService.PostStatementConfiguration(statementConfigurationDTO);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to post StatementConfiguration in Database: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to post StatementConfiguration in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while creating new StatementConfiguration: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPost("column-configuration")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> PostColumnConfiguration([FromBody] ColumnConfigurationDTO columnConfigurationDTO)
        {
            if (columnConfigurationDTO == null)
            {
                logger.LogWarning("ColumnConfiguration creation request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            try
            {
                var dbResponse = await statementConfigurationService.PostColumnConfiguration(columnConfigurationDTO);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to post ColumnConfiguration in Database: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to post ColumnConfiguration in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while creating new ColumnConfiguration: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }
    }
}