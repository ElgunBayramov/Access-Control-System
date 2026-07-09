using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities;
using Project.WebUI.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

[Area("Guard")]
[Authorize(Roles = "muhafize,admin")]
[Route("Guard")]
public class GuardController : Controller
{
    private readonly ProjectDbContext db;

    public GuardController(ProjectDbContext db)
    {
        this.db = db;
    }

    // /guard  →  Watch-a yönləndir
    public IActionResult Index() => RedirectToAction("Watch");

    // /guard/watch  →  Ana səhifə
    [HttpGet("watch")]
    public async Task<IActionResult> Watch()
    {
        var today = DateTime.Today;

        var logs = await db.AccessLogs
            .Include(l => l.User)
            .Where(l => l.Timestamp.Date == today)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();

        var permissions = await db.Permissions
            .Include(p => p.ProjectUser)
            .Where(p => p.DeletedDate == null
                   && p.Date.HasValue
                   && p.Date.Value.Date == today)
            .OrderByDescending(p => p.Id)
            .ToListAsync();

        return View(new GuardWatchViewModel
        {
            AccessLogs = logs,
            Permissions = permissions
        });
    }

    // /guard/doscan?token=xxx
    [HttpGet("doscan")]
    public async Task<IActionResult> DoScan(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Json(new { success = false, message = "Token boşdur" });

        var permission = await db.Permissions
            .Include(p => p.ProjectUser)
            .FirstOrDefaultAsync(p => p.QrToken == token);

        if (permission == null)
            return Json(new { success = false, message = "❌ Belə icazə mövcud deyil" });

        if (permission.Status != Status.Approved)
            return Json(new { success = false, message = "❌ Bu icazə təsdiqlənməyib" });

        if (permission.Date.HasValue)
        {
            double hours = TryParseHours(permission.Duration);
            DateTime expiry = permission.Date.Value.AddHours(hours);

            if (DateTime.Now > expiry)
                return Json(new
                {
                    success = false,
                    message = $"⏰ Müddəti bitib: {expiry:dd.MM.yyyy HH:mm}"
                });

            if (DateTime.Now < permission.Date.Value)
                return Json(new
                {
                    success = false,
                    message = $"⏳ Gəliş vaxtı hələ gəlməyib: {permission.Date.Value:dd.MM.yyyy HH:mm}"
                });
        }

        var log = new AccessLog
        {
            UserId = permission.ProjectUserId,
            Timestamp = DateTime.Now,
            Direction = "IN",
            ScannedBy = User.Identity.Name,
            Note = $"{permission.Name} {permission.Surname}"
        };
        db.AccessLogs.Add(log);
        await db.SaveChangesAsync();

        return Json(new
        {
            success = true,
            name = $"{permission.Name} {permission.Surname}",
            inviter = $"{permission.ProjectUser?.Name} {permission.ProjectUser?.Surname}",
            reason = permission.Reason,
            validUntil = permission.Date?.AddHours(TryParseHours(permission.Duration))
                             .ToString("HH:mm — dd.MM.yyyy")
        });
    }

    private double TryParseHours(string duration)
    {
        if (string.IsNullOrWhiteSpace(duration)) return 1;
        var cleaned = duration.ToLower()
            .Replace("saat", "").Replace("hour", "").Trim();
        return double.TryParse(cleaned, out double h) ? h : 1;
    }
}