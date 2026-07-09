using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.WebUI.AppCode.Extensions;
using Project.WebUI.AppCode.Services;
using Project.WebUI.Business.PermissionModule;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities;
using Project.WebUI.Models.Entities.Membership;
using System;
using System.Threading.Tasks;

namespace Project.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class PermissionsController : Controller
    {
        private readonly IMediator mediator;
        private readonly ProjectDbContext db;
        private readonly IEmailService emailService;
        private readonly IQrCodeService qrCodeService;
        private readonly UserManager<ProjectUser> userManager;

        public PermissionsController(
            IMediator mediator,
            ProjectDbContext db,
            IEmailService emailService,
            IQrCodeService qrCodeService,
            UserManager<ProjectUser> userManager)
        {
            this.mediator = mediator;
            this.db = db;
            this.emailService = emailService;
            this.qrCodeService = qrCodeService;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(PermissionPagedQuery query)
        {
            var response = await mediator.Send(query);
            if (Request.IsAjaxRequest())
                return PartialView("_ListBody", response);
            return View(response);
        }

        public async Task<IActionResult> Details(PermissionSingleQuery query)
        {
            var response = await mediator.Send(query);
            if (response == null)
                return NotFound();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(
            int UserId,
            string AdditionalParam,
            string RejectReason = null)
        {
            var permission = await db.Permissions
                .Include(p => p.ProjectUser)
                .FirstOrDefaultAsync(p => p.Id == UserId);

            if (permission == null)
                return NotFound("İcazə tapılmadı");

            switch (AdditionalParam)
            {
                case "Accept":
                    {
                        permission.Status = Status.Approved;

                        // ✅ QR token yarat
                        string qrToken = Guid.NewGuid().ToString("N");
                        permission.QrToken = qrToken;

                        // ✅ QR şəklini yarat
                        string qrFileName = $"perm-{permission.Id}-{Guid.NewGuid():N}.png";

                        // QR məzmunu: token + bitmə vaxtı
                        DateTime expiry = permission.Date?.AddHours(
                            TryParseHours(permission.Duration)) ?? DateTime.Now.AddHours(1);

                        string qrContent = qrToken;
                        string qrPath = qrCodeService.GenerateAndSave(qrContent, qrFileName);
                        permission.QrCodePath = qrPath;

                        await db.SaveChangesAsync();

                        // ✅ Kənar adamın emailinə QR göndər
                        string inviterName = $"{permission.ProjectUser?.Name} " +
                                            $"{permission.ProjectUser?.Surname}";
                        string expiryStr = expiry.ToString("dd.MM.yyyy HH:mm");
                        string startStr = permission.Date?.ToString("dd.MM.yyyy HH:mm") ?? "-";

                        await emailService.SendApproveEmailAsync(
     permission.Email,
     $"{permission.Name} {permission.Surname}",
     inviterName,
     permission.Reason ?? "-",
     startStr,
     expiryStr,
     qrPath);

                        return Ok(new { message = "Təsdiqləndi və email göndərildi" });
                    }

                case "Refuse":
                    {
                        permission.Status = Status.Rejected;
                        permission.RejectReason = RejectReason;
                        await db.SaveChangesAsync();

                        // ✅ İşçiyə reject emaili göndər
                        if (permission.ProjectUser?.Email != null)
                        {
                            await emailService.SendRejectEmailAsync(
    permission.ProjectUser.Email,
    $"{permission.ProjectUser.Name} {permission.ProjectUser.Surname}",
    $"{permission.Name} {permission.Surname}",
    RejectReason);
                        }

                        return Ok(new { message = "Rədd edildi və email göndərildi" });
                    }

                default:
                    return BadRequest("Naməlum əməliyyat");
            }
        }

        // "3 saat", "2" kimi string-dən saat sayını çıxar
        private double TryParseHours(string duration)
        {
            if (string.IsNullOrWhiteSpace(duration)) return 1;
            var cleaned = duration.ToLower()
                .Replace("saat", "")
                .Replace("hour", "")
                .Trim();
            return double.TryParse(cleaned, out double h) ? h : 1;
        }
    }
}