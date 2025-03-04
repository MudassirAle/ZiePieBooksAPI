using Application.Interface.QBDesktop;
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
    public class QBAccountController : ControllerBase
    {
        private readonly IQBAccountService qbAccountService;
        private readonly ILogger<QBAccountController> logger;

        public QBAccountController(IQBAccountService qbAccountService, ILogger<QBAccountController> logger)
        {
            this.qbAccountService = qbAccountService ?? throw new ArgumentNullException(nameof(qbAccountService), "QBAccount service is null");
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        [HttpGet("ticket/{ticket}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetByTicket(string ticket)
        {
            try
            {
                var response = await qbAccountService.GetByTicket(ticket);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve QBAccount with Ticket {ticket}: {response.ErrorMessage}");
                    return NotFound(response);
                }

                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching QBAccount with Ticket {ticket}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }
    }
}