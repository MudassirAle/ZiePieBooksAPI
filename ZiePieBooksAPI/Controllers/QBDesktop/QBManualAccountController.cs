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
    public class QBManualAccountController : ControllerBase
    {
        private readonly IQBManualAccountService qbManualAccountService;
        private readonly ILogger<QBManualAccountController> logger;

        public QBManualAccountController(IQBManualAccountService qbManualAccountService, ILogger<QBManualAccountController> logger)
        {
            this.qbManualAccountService = qbManualAccountService ?? throw new ArgumentNullException(nameof(qbManualAccountService), "QBManualAccount service is null");
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        [HttpGet("businessId/{businessId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetByBusinessId(int businessId)
        {
            try
            {
                var response = await qbManualAccountService.GetByBusinessId(businessId);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve QBManualAccount with BusinessId {businessId}: {response.ErrorMessage}");
                    return NotFound(response);
                }

                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching QBManualAccount with BusinessId {businessId}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPost]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Post([FromBody] QBManualAccount qbManualAccount)
        {
            if (qbManualAccount == null)
            {
                logger.LogWarning("QBManualAccount creation request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            try
            {
                var dbResponse = await qbManualAccountService.Post(qbManualAccount);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to post QBManualAccount in Database: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to post QBManualAccount in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while creating new QBManualAccount: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }
    }
}