namespace Project.WebUI.AppCode.Services
{
    public interface IQrCodeService
    {
        
        string GenerateAndSave(string token, string fileName);

     
        bool Validate(string token);
    }
}