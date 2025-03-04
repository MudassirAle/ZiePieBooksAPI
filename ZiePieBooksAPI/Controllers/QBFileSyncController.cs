using Application.Interface;
using Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Newtonsoft.Json;
using System.Text;
using ZiePieBooksAPI.Helper;

namespace AplosMigrationAPI.Controllers
{
    [Authorize]
    [EnableCors("ZiePieBooksCorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class QBFileSyncController : ControllerBase
    {
        private readonly IQBFileSyncService qBFileSyncService;
        private readonly IBusinessService businessService;
        private readonly IWCOutboxService wCOutboxService;
        private readonly ILogger<QBFileSyncController> logger;
        private readonly string _passphrase;

        public QBFileSyncController(
            IQBFileSyncService qBFileSyncService,
            IBusinessService businessService,
            IWCOutboxService wCOutboxService,
            ILogger<QBFileSyncController> logger,
            IConfiguration configuration
            )
        {
            this.qBFileSyncService = qBFileSyncService ?? throw new ArgumentNullException(nameof(qBFileSyncService));
            this.businessService = businessService ?? throw new ArgumentNullException(nameof(businessService));
            this.wCOutboxService = wCOutboxService ?? throw new ArgumentNullException(nameof(wCOutboxService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _passphrase = configuration["EncryptionSettings:Passphrase"] ?? throw new ArgumentNullException(nameof(configuration));
            if (string.IsNullOrEmpty(_passphrase))
            {
                throw new ArgumentException("Passphrase is missing or empty in appsettings.json", nameof(configuration));
            }
        }

        [HttpPost("receive")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Post([FromBody] QBFileReceive qBFileReceive)
        {
            if (qBFileReceive == null)
            {
                logger.LogWarning("QBFile creation request body is null.");
                return BadRequest(ResponseHelper.CreateErrorResponse<object>("Request body cannot be null."));
            }

            try
            {
                if (!string.IsNullOrEmpty(qBFileReceive.Password))
                {
                    qBFileReceive.Password = EncryptionHelper.Encrypt(qBFileReceive.Password, _passphrase);
                }

                var qBFileReceiveLog = new QBFileReceiveLog
                {
                    BusinessId = qBFileReceive.BusinessId,
                    Ticket = Guid.NewGuid().ToString(),
                    RequestType = "Receive",
                    BlobUri = qBFileReceive.BlobUri,
                    Password = qBFileReceive.Password,
                    ReceivedAt = DateTime.UtcNow,
                    Status = "Queued"
                };

                var dbResponse = await qBFileSyncService.Post(qBFileReceiveLog);
                if (!dbResponse.IsSuccess)
                {
                    logger.LogError($"Failed to post QBFile in Database: {dbResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to post QBFile in Database."));
                }

                var hierarchyResponse = await businessService.GetHierarchy(qBFileReceiveLog.BusinessId);

                if (!hierarchyResponse.IsSuccess)
                {
                    logger.LogError($"Failed to retrieve hierarchy: {hierarchyResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to retrieve hierarchy."));
                }

                var qBFileSyncRequest = new QBFileSyncRequest
                {
                    Hierarchy = hierarchyResponse.Data!,
                    Date = DateTime.UtcNow,
                    Ticket = qBFileReceiveLog.Ticket,
                    RequestType = qBFileReceiveLog.RequestType,
                    BlobUri = qBFileReceiveLog.BlobUri
                };

                var qBFileReceiveUpdateLog = new QBFileSyncUpdate
                {
                    Ticket = qBFileSyncRequest.Ticket,
                };

                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMinutes(5);
                    var requestUrl = "http://13.90.56.148:8080/api/QBFileSync/sync";
                    //var requestUrl = "https://localhost:44391/api/QBFileSync/sync";
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(qBFileSyncRequest), Encoding.UTF8, "application/json");
                    var httpResponse = await httpClient.PostAsync(requestUrl, jsonContent);

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();
                        var responseObject = JsonConvert.DeserializeObject<ServiceResponse<string>>(responseContent);

                        if (responseObject != null && responseObject.Data != null)
                        {
                            qBFileReceiveUpdateLog.FileAddress = responseObject.Data;
                            qBFileReceiveUpdateLog.ProcessedAt = DateTime.UtcNow;
                            qBFileReceiveUpdateLog.Status = "Processed";
                        }
                        else
                        {
                            qBFileReceiveUpdateLog.Status = "Fail";
                            qBFileReceiveUpdateLog.Error = "File Address is null or empty.";
                            logger.LogError($"Failed to receive valid File Address from response: {responseContent}");
                        }
                    }
                    else
                    {
                        qBFileReceiveUpdateLog.Status = "Fail";
                        qBFileReceiveUpdateLog.Error = httpResponse.ReasonPhrase ?? "Unknown error";
                        logger.LogError($"Failed to send QBFileOutboxRequest to {requestUrl}: {httpResponse.ReasonPhrase}");
                    }
                }

                var updateResponse = await qBFileSyncService.UpdateStatus(qBFileReceiveUpdateLog);

                if (!updateResponse.IsSuccess)
                {
                    logger.LogError($"Failed to update QBFileSync with ticket {qBFileReceiveLog.Ticket}: {updateResponse.ErrorMessage}");
                    return StatusCode(500, ResponseHelper.CreateErrorResponse<object>($"Failed to update QBFileSync record."));
                }

                //CompanyFileName = Path.GetFileName(qBFileReceiveUpdateLog.FileAddress),
                //CompanyFileLocation = Path.GetDirectoryName(qBFileReceiveUpdateLog.FileAddress)!,

                var wCOutbox = new WCOutbox
                {
                    Ticket = qBFileReceiveLog.Ticket,
                    UserName = "muddassir",
                    Password = "123456",
                    CompanyFileName = qBFileReceiveUpdateLog.FileAddress.Substring(qBFileReceiveUpdateLog.FileAddress.LastIndexOf('\\') + 1),
                    CompanyFileLocation = qBFileReceiveUpdateLog.FileAddress.Substring(0, qBFileReceiveUpdateLog.FileAddress.LastIndexOf('\\')),
                    Requests = GenerateWCRequests()
                };

                var wCOutboxResponse = await wCOutboxService.Post(wCOutbox);
                if (!wCOutboxResponse.IsSuccess)
                {
                    logger.LogError($"Failed to post WCOutbox in Database: {wCOutboxResponse.ErrorMessage}");
                    return BadRequest(ResponseHelper.CreateErrorResponse<object>("Failed to post WCOutbox in Database."));
                }

                return Ok(ResponseHelper.CreateSuccessResponse(dbResponse.Data));
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while creating new QBFile: {ex.Message}");
                return StatusCode(500, ResponseHelper.CreateErrorResponse<object>("An error occurred while processing your request: " + ex.Message));
            }
        }
        private List<WCRequest> GenerateWCRequests()
        {
            return new List<WCRequest>
            {
                new WCRequest
                {
                    Entity = "Account",
                    RequestType = "Read",
                    RequestXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?><?qbxml version=\"16.0\"?><QBXML><QBXMLMsgsRq onError=\"continueOnError\"><AccountQueryRq requestID=\"1\"></AccountQueryRq></QBXMLMsgsRq></QBXML>"
                },
                //new WCRequest
                //{
                //    Entity = "Check",
                //    RequestType = "Read",
                //    RequestXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?><?qbxml version=\"16.0\"?><QBXML><QBXMLMsgsRq onError=\"continueOnError\"><CheckQueryRq requestID=\"2\"><IncludeLineItems >true</IncludeLineItems></CheckQueryRq></QBXMLMsgsRq></QBXML>"
                //},
                new WCRequest
                {
                    Entity = "Customer",
                    RequestType = "Read",
                    RequestXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?><?qbxml version=\"16.0\"?><QBXML><QBXMLMsgsRq onError=\"continueOnError\"><CustomerQueryRq requestID=\"2\"></CustomerQueryRq></QBXMLMsgsRq></QBXML>"
                },
                //new WCRequest
                //{
                //    Entity = "Deposit",
                //    RequestType = "Read",
                //    RequestXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?><?qbxml version=\"16.0\"?><QBXML><QBXMLMsgsRq onError=\"continueOnError\"><DepositQueryRq requestID=\"4\"><IncludeLineItems >true</IncludeLineItems></DepositQueryRq></QBXMLMsgsRq></QBXML>"
                //},
                new WCRequest
                {
                    Entity = "Employee",
                    RequestType = "Read",
                    RequestXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?><?qbxml version=\"16.0\"?><QBXML><QBXMLMsgsRq onError=\"continueOnError\"><EmployeeQueryRq requestID=\"3\"></EmployeeQueryRq></QBXMLMsgsRq></QBXML>"
                },
                //new WCRequest
                //{
                //    Entity = "JournalEntry",
                //    RequestType = "Read",
                //    RequestXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?><?qbxml version=\"16.0\"?><QBXML><QBXMLMsgsRq onError=\"continueOnError\"><JournalEntryQueryRq requestID=\"6\"><IncludeLineItems >true</IncludeLineItems></JournalEntryQueryRq></QBXMLMsgsRq></QBXML>"
                //},
                //new WCRequest
                //{
                //    Entity = "SalesReceipt",
                //    RequestType = "Read",
                //    RequestXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?><?qbxml version=\"16.0\"?><QBXML><QBXMLMsgsRq onError=\"continueOnError\"><SalesReceiptQueryRq requestID=\"7\"></SalesReceiptQueryRq></QBXMLMsgsRq></QBXML>"
                //},
                //new WCRequest
                //{
                //    Entity = "Transfer",
                //    RequestType = "Read",
                //    RequestXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?><?qbxml version=\"16.0\"?><QBXML><QBXMLMsgsRq onError=\"continueOnError\"><TransferQueryRq requestID=\"8\"></TransferQueryRq></QBXMLMsgsRq></QBXML>"
                //},
                new WCRequest
                {
                    Entity = "Vendor",
                    RequestType = "Read",
                    RequestXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?><?qbxml version=\"16.0\"?><QBXML><QBXMLMsgsRq onError=\"continueOnError\"><VendorQueryRq requestID=\"4\"></VendorQueryRq></QBXMLMsgsRq></QBXML>"
                },
                new WCRequest
                {
                    Entity = "TransactionReport",
                    RequestType = "Read",
                    RequestXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?><?qbxml version=\"16.0\"?><QBXML><QBXMLMsgsRq onError=\"continueOnError\"><CustomDetailReportQueryRq requestID=\"5\"><CustomDetailReportType>CustomTxnDetail</CustomDetailReportType><ReportDateMacro>All</ReportDateMacro><SummarizeRowsBy>Account</SummarizeRowsBy><IncludeColumn>Account</IncludeColumn><IncludeColumn>Amount</IncludeColumn><IncludeColumn>Class</IncludeColumn><IncludeColumn>ClearedStatus</IncludeColumn><IncludeColumn>Credit</IncludeColumn><IncludeColumn>Date</IncludeColumn><IncludeColumn>Debit</IncludeColumn><IncludeColumn>Item</IncludeColumn><IncludeColumn>ItemDesc</IncludeColumn><IncludeColumn>Memo</IncludeColumn><IncludeColumn>ModifiedTime</IncludeColumn><IncludeColumn>Name</IncludeColumn><IncludeColumn>NameAccountNumber</IncludeColumn><IncludeColumn>OpenBalance</IncludeColumn><IncludeColumn>RefNumber</IncludeColumn><IncludeColumn>RunningBalance</IncludeColumn><IncludeColumn>SplitAccount</IncludeColumn><IncludeColumn>TxnID</IncludeColumn><IncludeColumn>TxnNumber</IncludeColumn><IncludeColumn>TxnType</IncludeColumn></CustomDetailReportQueryRq></QBXMLMsgsRq></QBXML>"
                }
            };
        }
    }
}