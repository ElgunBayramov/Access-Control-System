namespace Project.WebUI.AppCode.Services
{
    public interface ICryptoService
    {
        string Decrypt(string value);
        string Encrypt(string value, bool appliedUrlEncode = false);
        string ToMd5(string value);
    }
}