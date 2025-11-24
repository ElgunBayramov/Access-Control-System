using Project.WebUI.Models.Entities.Membership;

namespace Project.WebUI.AppCode.Services
{
    public interface ITokenService
    {
        string BuildToken(ProjectUser user);
        bool ValidateToken(string token);
    }
}