using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Project.WebUI.AppCode.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private SendGridClient CreateClient()
            => new SendGridClient(configuration["SendGrid:ApiKey"]);

        private EmailAddress FromAddress => new EmailAddress(
            configuration["SendGrid:FromEmail"],
            configuration["SendGrid:FromName"]);

        private async Task<Attachment> BuildQrAttachment(string qrFilePath)
        {
            var fullPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                qrFilePath.TrimStart('/'));

            if (!File.Exists(fullPath))
                throw new Exception($"QR file not found: {fullPath}");

            var bytes = await File.ReadAllBytesAsync(fullPath);
            var base64 = Convert.ToBase64String(bytes);

            return new Attachment
            {
                Content = base64,
                Filename = "qr.png",
                Type = "image/png",
                Disposition = "inline",
                ContentId = "qrcode"
            };
        }
        public async Task<bool> SendEmailAsync(string toEmail, string approveLink)
        {
            var client = CreateClient();
            var msg = MailHelper.CreateSingleEmail(
                FromAddress,
                new EmailAddress(toEmail),
                "Verification Email",
                "",
                $"Please approve: <a href='{approveLink}'>link</a>");
            var response = await client.SendEmailAsync(msg);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendEmailAsync(
            string toEmail, string subject, string message)
        {
            var client = CreateClient();
            var msg = MailHelper.CreateSingleEmail(
                FromAddress,
                new EmailAddress(toEmail),
                subject,
                message,
                message);
            var response = await client.SendEmailAsync(msg);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendEmailWithQrAsync(
            string toEmail, string subject,
            string message, string qrFilePath)
        {
            var client = CreateClient();
            var msg = MailHelper.CreateSingleEmail(
                FromAddress,
                new EmailAddress(toEmail),
                subject, "", "");

            msg.AddAttachment(await BuildQrAttachment(qrFilePath));
            msg.HtmlContent = $@"
                <p>{message}</p>
                <p>Your QR:</p>
                <img src='cid:qrcode' width='200' height='200'/>
                <p>Show in the entry.</p>";

            var response = await client.SendEmailAsync(msg);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendWelcomeEmailAsync(
            string toEmail, string fullName,
            string username, string password)
        {
            var client = CreateClient();
            var msg = MailHelper.CreateSingleEmail(
                FromAddress,
                new EmailAddress(toEmail),
                "Welcome to — A.C.SYSTEM",
                "", "");

            msg.HtmlContent = $@"
<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'>
<style>
  body{{font-family:'Segoe UI',Arial,sans-serif;background:#f4f6f9;margin:0;padding:0;}}
  .wrap{{max-width:560px;margin:40px auto;background:#fff;border-radius:12px;
         overflow:hidden;box-shadow:0 4px 20px rgba(0,0,0,.08);}}
  .hdr{{background:linear-gradient(135deg,#1a1a2e,#16213e);
        padding:36px 40px;text-align:center;}}
  .hdr h1{{color:#fff;margin:0;font-size:1.6rem;letter-spacing:3px;}}
  .hdr p{{color:rgba(255,255,255,.6);margin:6px 0 0;font-size:.85rem;}}
  .bdy{{padding:36px 40px;}}
  .box{{background:#f8f9ff;border:1px solid #e0e4ff;border-radius:10px;
        padding:20px 24px;margin-bottom:24px;}}
  .row{{display:flex;justify-content:space-between;
        padding:10px 0;border-bottom:1px solid #eee;}}
  .row:last-child{{border-bottom:none;}}
  .lbl{{color:#888;font-size:.9rem;}}
  .val{{color:#222;font-weight:600;font-size:.95rem;}}
  .pwd{{color:#e74c3c;font-size:1.05rem;letter-spacing:1px;}}
  .warn{{background:#fff8e1;border-left:4px solid #f39c12;
         padding:12px 16px;border-radius:6px;
         font-size:.85rem;color:#7d6300;margin-bottom:24px;}}
  .ftr{{background:#f4f6f9;padding:20px 40px;text-align:center;
        font-size:.8rem;color:#aaa;}}
</style></head>
<body>
<div class='wrap'>
  <div class='hdr'>
    <h1>🔒 A.C.SYSTEM</h1>
    <p>Access Control System</p>
  </div>
  <div class='bdy'>
    <p style='font-size:1.1rem;color:#333;margin-bottom:20px;'>
      Dear <strong>{fullName}</strong>, welcome!
    </p>
    <p style='color:#555;margin-bottom:20px;font-size:.95rem;'>
      Your account has been successfully created. You can log in with the following information:
    </p>
    <div class='box'>
      <div class='row'>
        <span class='lbl'>Name and Surname</span>
        <span class='val'>{fullName}</span>
      </div>
      <div class='row'>
        <span class='lbl'>Email address</span>
        <span class='val'>{toEmail}</span>
      </div>
      <div class='row'>
        <span class='lbl'>Username</span>
        <span class='val'>{username}</span>
      </div>
      <div class='row'>
        <span class='lbl'>Password</span>
        <span class='val pwd'>{password}</span>
      </div>
    </div>
    <div class='warn'>
      ⚠️ For security reasons, we recommend that you change your password after your first login.
    </div>
    <p style='color:#888;font-size:.85rem;'>
      If you experience any problems, please contact your system administrator.
    </p>
  </div>
  <div class='ftr'>© 2024 A.C.SYSTEM · This email was sent automatically</div>
</div>
</body></html>";

            var response = await client.SendEmailAsync(msg);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendApproveEmailAsync(
            string toEmail, string guestName,
            string inviterName, string reason,
            string arrivalTime, string expiryTime,
            string qrImagePath)
        {
            var client = CreateClient();
            var msg = MailHelper.CreateSingleEmail(
                FromAddress,
                new EmailAddress(toEmail),
                "✅ Your university admission has been approved.",
                "", "");

            msg.AddAttachment(await BuildQrAttachment(qrImagePath));

            msg.HtmlContent = $@"
<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'>
<style>
  body{{font-family:'Segoe UI',Arial,sans-serif;background:#f4f6f9;margin:0;padding:0;}}
  .wrap{{max-width:560px;margin:40px auto;background:#fff;border-radius:12px;
         overflow:hidden;box-shadow:0 4px 20px rgba(0,0,0,.08);}}
  .hdr{{background:linear-gradient(135deg,#1a6b3a,#27ae60);
        padding:36px 40px;text-align:center;}}
  .hdr h1{{color:#fff;margin:0;font-size:1.5rem;letter-spacing:2px;}}
  .hdr p{{color:rgba(255,255,255,.75);margin:6px 0 0;font-size:.85rem;}}
  .bdy{{padding:36px 40px;}}
  .box{{background:#f0fff4;border:1px solid #b7ebc8;border-radius:10px;
        padding:20px 24px;margin-bottom:24px;}}
  .row{{display:flex;justify-content:space-between;
        padding:9px 0;border-bottom:1px solid #d4f0de;}}
  .row:last-child{{border-bottom:none;}}
  .lbl{{color:#5a8a6a;font-size:.9rem;}}
  .val{{color:#1a4a2a;font-weight:600;font-size:.95rem;}}
  .qr{{text-align:center;margin:24px 0;}}
  .qr p{{color:#555;font-size:.9rem;margin-bottom:12px;}}
  .warn{{background:#fff8e1;border-left:4px solid #f39c12;
         padding:12px 16px;border-radius:6px;
         font-size:.85rem;color:#7d6300;margin-top:20px;}}
  .ftr{{background:#f4f6f9;padding:20px 40px;text-align:center;
        font-size:.8rem;color:#aaa;}}
</style></head>
<body>
<div class='wrap'>
  <div class='hdr'>
    <h1>✅ Your Permission Approved</h1>
    <p>A.C.SYSTEM — Access Control</p>
  </div>
  <div class='bdy'>
    <p style='font-size:1.05rem;color:#333;margin-bottom:20px;'>
      Dear <strong>{guestName}</strong>,
    </p>
    <p style='color:#555;margin-bottom:20px;font-size:.95rem;'>
      Your university entrance permit has been approved.
      Please show the QR code below to the security guard at the entrance.
    </p>
    <div class='box'>
      <div class='row'>
        <span class='lbl'>Inviter</span>
        <span class='val'>{inviterName}</span>
      </div>
      <div class='row'>
        <span class='lbl'>Reason</span>
        <span class='val'>{reason}</span>
      </div>
      <div class='row'>
        <span class='lbl'>Arrival time</span>
        <span class='val'>{arrivalTime}</span>
      </div>
      <div class='row'>
        <span class='lbl'>Expiry time</span>
        <span class='val'>{expiryTime}</span>
      </div>
    </div>
    <div class='qr'>
      <p>📱 Show this QR code to the security guard:</p>
      <img src='cid:qrcode'
           width='200' height='200'
           style='border:4px solid #b7ebc8;border-radius:12px;padding:8px;'/>
    </div>
    <div class='warn'>
      ⚠️ This QR code is only valid between <strong>{arrivalTime} — {expiryTime}</strong>. Do not share it with anyone else.
    </div>
  </div>
  <div class='ftr'>© 2024 A.C.SYSTEM · This email was sent automatically.</div>
</div>
</body></html>";

            var response = await client.SendEmailAsync(msg);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendRejectEmailAsync(
            string toEmail, string workerName,
            string guestName, string rejectReason)
        {
            var client = CreateClient();
            var msg = MailHelper.CreateSingleEmail(
                FromAddress,
                new EmailAddress(toEmail),
                "❌ Permit application denied",
                "", "");

            msg.HtmlContent = $@"
<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'>
<style>
  body{{font-family:'Segoe UI',Arial,sans-serif;background:#f4f6f9;margin:0;padding:0;}}
  .wrap{{max-width:560px;margin:40px auto;background:#fff;border-radius:12px;
         overflow:hidden;box-shadow:0 4px 20px rgba(0,0,0,.08);}}
  .hdr{{background:linear-gradient(135deg,#8b1a1a,#e74c3c);
        padding:36px 40px;text-align:center;}}
  .hdr h1{{color:#fff;margin:0;font-size:1.5rem;letter-spacing:2px;}}
  .hdr p{{color:rgba(255,255,255,.75);margin:6px 0 0;font-size:.85rem;}}
  .bdy{{padding:36px 40px;}}
  .box{{background:#fff5f5;border:1px solid #fcc;border-radius:10px;
        padding:20px 24px;margin:20px 0;}}
  .row{{display:flex;justify-content:space-between;
        padding:9px 0;border-bottom:1px solid #ffe0e0;}}
  .row:last-child{{border-bottom:none;}}
  .lbl{{color:#a05050;font-size:.9rem;}}
  .val{{color:#5a1a1a;font-weight:600;font-size:.95rem;}}
  .reason{{background:#fff8f8;border-left:4px solid #e74c3c;
           padding:14px 18px;border-radius:6px;
           margin:20px 0;font-size:.95rem;color:#5a1a1a;}}
  .ftr{{background:#f4f6f9;padding:20px 40px;text-align:center;
        font-size:.8rem;color:#aaa;}}
</style></head>
<body>
<div class='wrap'>
  <div class='hdr'>
    <h1>❌ Permission Denied</h1>
    <p>A.C.SYSTEM — Access Control</p>
  </div>
  <div class='bdy'>
    <p style='font-size:1.05rem;color:#333;'>
      Dear <strong>{workerName}</strong>,
    </p>
    <p style='color:#555;margin:12px 0;font-size:.95rem;'>
    Your permission request for the following person has been denied:
    </p>
    <div class='box'>
      <div class='row'>
        <span class='lbl'>Guest</span>
        <span class='val'>{guestName}</span>
      </div>
    </div>
    <div class='reason'>
      <strong>Reject reason:</strong><br/>
      {rejectReason ?? "Not shown"}
    </div>
    <p style='color:#888;font-size:.85rem;margin-top:16px;'>
      If you wish to reapply, you can fill out a new application through the system.
    </p>
  </div>
  <div class='ftr'>© 2024 A.C.SYSTEM · This email was sent automatically.</div>
</div>
</body></html>";

            var response = await client.SendEmailAsync(msg);
            return response.IsSuccessStatusCode;
        }
    }
}