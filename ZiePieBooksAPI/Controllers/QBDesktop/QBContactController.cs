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
    public class QBContactController : ControllerBase
    {
        private readonly IQBContactService qbContactService;
        private readonly ILogger<QBContactController> logger;

        public QBContactController(IQBContactService qbContactService, ILogger<QBContactController> logger)
        {
            this.qbContactService = qbContactService ?? throw new ArgumentNullException(nameof(qbContactService), "QBContact service is null");
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        [HttpGet("ticket/{ticket}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetByTicket(string ticket)
        {
            try
            {
                var response = await qbContactService.GetByTicket(ticket);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve QBContact with Ticket {ticket}: {response.ErrorMessage}");
                    return NotFound(response);
                }

                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching QBContact with Ticket {ticket}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }
    }
}