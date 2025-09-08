namespace TscLoanManagement.TSCDB.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string username, string password);
        Task SendOtpEmailAsync(string toEmail, string otp);
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
