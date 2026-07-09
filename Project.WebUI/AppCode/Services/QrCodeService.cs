using Microsoft.AspNetCore.Hosting;
using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Project.WebUI.AppCode.Services
{
    public class QrCodeService : IQrCodeService
    {
        private readonly string _saveDirectory;

        public QrCodeService(IWebHostEnvironment env)
        {
            _saveDirectory = Path.Combine(
            env.WebRootPath,
            "uploads",
            "qrcodes"
        );

            if (!Directory.Exists(_saveDirectory))
                Directory.CreateDirectory(_saveDirectory);
        }

        public string GenerateAndSave(string token, string fileName)
        {
            using var qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(token, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrCodeData);
            using Bitmap qrBitmap = qrCode.GetGraphic(20);

            string fullPath = Path.Combine(_saveDirectory, fileName);
            qrBitmap.Save(fullPath, ImageFormat.Png);

            return "/uploads/qrcodes/" + fileName;
        }

        public bool Validate(string token)
        {
            return !string.IsNullOrWhiteSpace(token);
        }
    }
}