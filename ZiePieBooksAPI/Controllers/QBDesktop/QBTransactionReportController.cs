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
    public class QBTransactionReportController : ControllerBase
    {
        private readonly IQBTransactionReportService qbTransactionReportService;
        private readonly ILogger<QBTransactionReportController> logger;

        public QBTransactionReportController(IQBTransactionReportService qbTransactionReportService, ILogger<QBTransactionReportController> logger)
        {
            this.qbTransactionReportService = qbTransactionReportService ?? throw new ArgumentNullException(nameof(qbTransactionReportService), "QBTransactionReport service is null");
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        [HttpGet("ticket/{ticket}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetByTicket(string ticket, [FromQuery] string startDate, [FromQuery] string endDate)
        {
            try
            {
                var response = await qbTransactionReportService.GetByTicket(ticket, startDate, endDate);

                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve QBTransactionReport with Ticket {ticket}: {response.ErrorMessage}");
                    return NotFound(response);
                }

                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching QBTransactionReport with Ticket {ticket}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }
    }
}
