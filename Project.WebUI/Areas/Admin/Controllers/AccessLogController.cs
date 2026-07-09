using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities;
using Project.WebUI.Models.Entities.Membership;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Project.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,muhafize")]
    public class AccessLogController : Controller
    {
        private readonly ProjectDbContext db;
        private readonly UserManager<ProjectUser> userManager;

        public AccessLogController(
            ProjectDbContext db,
            UserManager<ProjectUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Scan(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Token boşdur");

            var user = await db.Users
                .FirstOrDefaultAsync(u => u.QrCodeToken == token);

            if (user == null)
                return NotFound("İstifadəçi tapılmadı");

            // Son loqa bax — IN idi çıxış, OUT idi giriş
            var lastLog = await db.AccessLogs
                .Where(l => l.UserId == user.Id)
                .OrderByDescending(l => l.Timestamp)
                .FirstOrDefaultAsync();

            string direction = (lastLog == null || lastLog.Direction == "OUT")
                ? "IN" : "OUT";

            var log = new AccessLog
            {
                UserId = user.Id,
                Timestamp = DateTime.Now,
                Direction = direction,
                ScannedBy = User.Identity.Name,
                Note = $"{user.Name} {user.Surname} — {direction}"
            };

            db.AccessLogs.Add(log);
            await db.SaveChangesAsync();

            // ✅ JSON cavab — mobil cihaz üçün
            return Json(new
            {
                success = true,
                name = $"{user.Name} {user.Surname}",
                finCode = user.FinCode,
                direction,
                time = log.Timestamp.ToString("HH:mm:ss dd.MM.yyyy"),
                imagePath = user.ImagePath
            });
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 20;
            var logs = await db.AccessLogs
                .Include(l => l.User)
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(logs);
        }
    }
}