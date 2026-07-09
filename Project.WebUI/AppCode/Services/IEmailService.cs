using System.Threading.Tasks;

namespace Project.WebUI.AppCode.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string approveLink);
        Task<bool> SendEmailAsync(string toEmail, string subject, string message);
        Task<bool> SendEmailWithQrAsync(string toEmail, string subject,
                                        string message, string qrImagePath);

     
        Task<bool> SendWelcomeEmailAsync(string toEmail, string fullName,
                                         string username, string password);
        Task<bool> SendApproveEmailAsync(string toEmail, string guestName,
                                          string inviterName, string reason,
                                          string arrivalTime, string expiryTime,
                                          string qrImagePath);
        Task<bool> SendRejectEmailAsync(string toEmail, string workerName,
                                         string guestName, string rejectReason);
    }
}