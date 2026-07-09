using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Project.WebUI.AppCode.Extensions;
using Project.WebUI.AppCode.Services;
using Project.WebUI.Models.Entities.Membership;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.AccountModule
{
    public class RegisterCommand : IRequest<ProjectUser>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public DateTime RegisterDate { get; set; }
        public string FinCode { get; set; }
        public IFormFile Image { get; set; }
        public int ProfessionId { get; set; }
        public int DepartmentId { get; set; }

        public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ProjectUser>
        {
            private readonly UserManager<ProjectUser> userManager;
            private readonly IActionContextAccessor ctx;
            private readonly IEmailService emailService;
            private readonly ICryptoService cryptoService;
            private readonly IQrCodeService qrCodeService;

            public RegisterCommandHandler(
                UserManager<ProjectUser> userManager,
                IActionContextAccessor ctx,
                IEmailService emailService,
                ICryptoService cryptoService,
                IQrCodeService qrCodeService)
            {
                this.userManager = userManager;
                this.ctx = ctx;
                this.emailService = emailService;
                this.cryptoService = cryptoService;
                this.qrCodeService = qrCodeService;
            }

            public async Task<ProjectUser> Handle(
                RegisterCommand request,
                CancellationToken cancellationToken)
            {
                var existingUser = await userManager.FindByEmailAsync(request.Email);

                if (existingUser != null)
                {
                    ctx.ActionContext.ModelState
                        .AddModelError("Email", "Bu e-poçt artıq istifadə olunub");
                    return null;
                }

                // 🔥 QR TOKEN
                string qrToken = Guid.NewGuid().ToString("N");

                var user = new ProjectUser
                {
                    Email = request.Email,
                    Name = request.Name,
                    Surname = request.Surname,
                    FinCode = request.FinCode,
                    EmailConfirmed = true,
                    RegisterDate = DateTime.Now,
                    ProfessionId = request.ProfessionId,
                    DepartmentId = request.DepartmentId,
                    UserName = $"{request.Name}-{Guid.NewGuid()}".ToLower(),
                    QrCodeToken = qrToken
                };

                // 📷 PROFILE IMAGE
                if (request.Image != null && request.Image.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);

                    var uploadsFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/uploads/images");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await request.Image.CopyToAsync(stream);

                    user.ImagePath = "/uploads/images/" + fileName;
                }

                // 👤 CREATE USER
                var result = await userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    foreach (var item in result.Errors)
                        ctx.ActionContext.ModelState.AddModelError("", item.Description);

                    return null;
                }

                // 🔥 QR FILE CREATE
                string qrFileName = $"qr-{user.Id}-{Guid.NewGuid():N}.png";

                string qrPath = qrCodeService.GenerateAndSave(qrToken, qrFileName);

                // 💾 SAVE TO DB
                user.QrCodePath = qrPath;
                await userManager.UpdateAsync(user);

                await emailService.SendWelcomeEmailAsync(
                    user.Email,
                    $"{user.Name} {user.Surname}",
                    user.UserName,
                    request.Password  // CreateAsync-dən əvvəl gəlir, hələ plain text
                );

                return user;
            }
        }
    }
}