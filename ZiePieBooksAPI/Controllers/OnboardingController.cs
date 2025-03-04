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
    public class OnboardingController : ControllerBase
    {
        private readonly IOnboardingService onboardingService;
        private readonly ILogger<OnboardingController> logger;
        private readonly string _passphrase;

        public OnboardingController(
            IOnboardingService onboardingService,
            ILogger<OnboardingController> logger,
            IConfiguration configuration)
        {
            this.onboardingService = onboardingService ?? throw new ArgumentNullException(nameof(onboardingService), "Onboarding service is null");
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger is null");
            _passphrase = configuration["EncryptionSettings:Passphrase"] ?? throw new ArgumentNullException(nameof(configuration));
            if (string.IsNullOrEmpty(_passphrase))
            {
                throw new ArgumentException("Passphrase is missing or empty in appsettings.json", nameof(configuration));
            }
        }

        [HttpGet("businessId/{businessId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetByBusinessID(int businessId)
        {
            try
            {
                var response = await onboardingService.GetByBusinessID(businessId);
                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve Onboarding with BusinessId {businessId}: {response.ErrorMessage}");
                    return NotFound(response);
                }
                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching Onboarding with BusinessId {businessId}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPost]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Post([FromBody] OnboardingDTO onboardingDTO)
        {
            if (onboardingDTO == null)
            {
                logger.LogWarning("Onboarding creation request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            try
            {
                var dbResponse = await onboardingService.Post(onboardingDTO);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to post Onboarding in Database: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to post Onboarding in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while creating new Onboarding: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPut("data-source")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> UpdateDataSource([FromBody] DataSourceDTO dataSourceDTO)
        {
            if (dataSourceDTO == null)
            {
                logger.LogWarning("DataSource update request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            try
            {
                var dbResponse = await onboardingService.UpdateDataSource(dataSourceDTO);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to update DataSource in Database with ID {dataSourceDTO.ID}: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to update DataSource in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while updating DataSource with ID {dataSourceDTO.ID}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpGet("admin")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetForAdmin()
        {
            try
            {
                var response = await onboardingService.GetForAdmin();

                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve admin-related onboarding data: {response.ErrorMessage}");
                    return NotFound(ResponseHelper.CreateErrorResponse<object>("Failed to retrieve admin-related onboarding data."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching admin-related onboarding data: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpPut("assign-to")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> AssignTo([FromBody] AssignmentDTO assignmentDTO)
        {
            if (assignmentDTO == null)
            {
                logger.LogWarning("Assignment update request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            if (assignmentDTO.ID <= 0 || assignmentDTO.AssignedToId <= 0)
            {
                logger.LogWarning("Invalid ID or AssignedToId.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Invalid ID or AssignedToId."));
            }

            try
            {
                // Call the onboarding service to update the AssignedToId
                var dbResponse = await onboardingService.UpdateAssignedToId(assignmentDTO);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to update AssignedToId for Onboarding with ID {assignmentDTO.ID}: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to update AssignedToId."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while updating AssignedToId for Onboarding with ID {assignmentDTO.ID}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }

        [HttpGet("subadmin/{assignedToId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<IActionResult> GetForSubAdmin(int assignedToId)
        {
            try
            {
                var response = await onboardingService.GetForSubAdmin(assignedToId);

                if (!response.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve sub-admin-related onboarding data for AssignedToId {assignedToId}: {response.ErrorMessage}");
                    return NotFound(ResponseHelper.CreateErrorResponse<object>("Failed to retrieve sub-admin-related onboarding data."));
                }
                if (response.Data != null)
                {
                    foreach (var onboarding in response.Data)
                    {
                        if (!string.IsNullOrEmpty(onboarding.Password))
                        {
                            onboarding.Password = EncryptionHelper.Decrypt(onboarding.Password, _passphrase);
                        }
                    }
                }

                //if (response.Data != null)
                //{
                //    // Use LINQ to decrypt the Password for each onboarding item
                //    response.Data
                //        .Where(onboarding => !string.IsNullOrEmpty(onboarding.Password))
                //        .ToList()
                //        .ForEach(onboarding => onboarding.Password = EncryptionHelper.Decrypt(onboarding.Password!, _passphrase));
                //}

                return Ok(ResponseHelper.CreateSuccessResponse(response.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while fetching sub-admin-related onboarding data for AssignedToId {assignedToId}: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }
    }
}