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
    public class EmailController : ControllerBase
    {
        private readonly IEmailService emailService;
        private readonly ILogger<EmailController> logger;

        public EmailController(IEmailService emailService, ILogger<EmailController> logger)
        {
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService), "Email service is null");
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        [HttpPost]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> SendEmail([FromBody] Email email)
        {
            if (email == null)
            {
                logger.LogWarning("Email request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            try
            {
                var response = await emailService.Send(email);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to send email: {response.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to send email."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while sending email: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }
    }
}