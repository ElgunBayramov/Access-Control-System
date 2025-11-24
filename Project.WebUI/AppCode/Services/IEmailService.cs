using System.Threading.Tasks;

namespace Project.WebUI.AppCode.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string approveLink);
        Task<bool> SendEmailAsync(string toEmail, string subject, string message);
    }
}