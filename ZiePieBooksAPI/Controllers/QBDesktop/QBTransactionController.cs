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
    public class QBTransactionController : ControllerBase
    {
        private readonly IQBTransactionService qbTransactionService;
        private readonly ILogger<QBTransactionController> logger;

        public QBTransactionController(IQBTransactionService qbTransactionService, ILogger<QBTransactionController> logger)
        {
            this.qbTransactionService = qbTransactionService ?? throw new ArgumentNullException(nameof(qbTransactionService), "QBTransaction service is null");
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        [HttpGet("ticket/{ticket}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetByTicket(string ticket, [FromQuery] string startDate, [FromQuery] string endDate)
        {
            try
            {
                var response = await qbTransactionService.GetByTicket(ticket, startDate, endDate);

                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve QBTransactions with Ticket {ticket}: {response.ErrorMessage}");
                    return NotFound(response);
                }

                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching QBTransactions with Ticket {ticket}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }
    }
}