namespace Core.Model
{
    public class QBFileReceive
    {
        public int BusinessId { get; set; }
        public string BlobUri { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class QBFileReceiveLog
    {
        public int ID { get; set; }
        public int BusinessId { get; set; }
        public string Ticket { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public string BlobUri { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime ReceivedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class Hierarchy
    {
        public string Tenant { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public string Business { get; set; } = string.Empty;
    }

    public class QBFileSyncRequest
    {
        public Hierarchy Hierarchy { get; set; } = new Hierarchy();
        public DateTime Date { get; set; }
        public string Ticket { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public string BlobUri { get; set; } = string.Empty;
    }

    public class QBFileSyncUpdate
    {
        public string Ticket { get; set; } = string.Empty;
        public string FileAddress { get; set; } = string.Empty;
        public DateTime? ProcessedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }

    public class WCOutbox
    {
        public int ID { get; set; }
        public string Ticket { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string CompanyFileName { get; set; } = string.Empty;
        public string CompanyFileLocation { get; set; } = string.Empty;
        public List<WCRequest> Requests { get; set; } = new List<WCRequest>();
        public string Status { get; set; } = "Pending";
    }

    public class WCRequest
    {
        public string Entity { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public string RequestXML { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string? Error { get; set; } = null;
    }
}
