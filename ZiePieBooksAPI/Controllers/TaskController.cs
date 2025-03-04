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
    public class TaskController : ControllerBase
    {
        private readonly ITaskService taskService;
        private readonly ILogger<TaskController> logger;

        public TaskController(ITaskService taskService, ILogger<TaskController> logger)
        {
            this.taskService = taskService ?? throw new ArgumentNullException(nameof(taskService), "Task service is null");
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
        }

        [HttpGet("onboardingId/{onboardingId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetByOnboardingId(int onboardingId)
        {
            try
            {
                var response = await taskService.GetByOnboardingId(onboardingId);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve Task with OnboardingId {onboardingId}: {response.ErrorMessage}");
                    return NotFound(response);
                }

                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching Task with OnboardingId {onboardingId}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPost("validate-password")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> ValidatePassword([FromBody] ValidatePasswordDTO validatePasswordDto)
        {
            if (validatePasswordDto == null)
            {
                logger.LogWarning("Password validation request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            try
            {
                var response = await taskService.ValidatePassword(validatePasswordDto);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to validate password: {response.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to validate password."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while validating password: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPut("enqueue-ticket/{ticket}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> EnqueueTicket(string ticket)
        {
            if (string.IsNullOrWhiteSpace(ticket))
            {
                logger.LogWarning("Queue ticket request body is null or empty.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Ticket cannot be null or empty."));
            }

            try
            {
                var response = await taskService.EnqueueTicket(ticket);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to queue ticket: {response.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to queue ticket."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while queuing ticket: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPut("update-status/{onboardingId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> UpdateStatus(int onboardingId)
        {
            if (onboardingId <= 0)
            {
                logger.LogWarning($"Invalid OnboardingId: {onboardingId}.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Invalid OnboardingId."));
            }

            try
            {
                var response = await taskService.UpdateStatus(onboardingId);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to update status for OnboardingId {onboardingId}: {response.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to update status."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while updating status for OnboardingId {onboardingId}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }
    }
}
