namespace TscLoanManagement.TSCDB.Core.Domain.Authentication
{
    public class MailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SenderEmail { get; set; }
        public bool EnableSSL { get; set; }
    }
}
