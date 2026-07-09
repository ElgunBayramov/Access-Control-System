using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.WebUI.Business.PermissionModule;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities;
using Project.WebUI.Models.Entities.Membership;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.WebUI.Controllers
{
    public class HomeController : Controller

    {
        private readonly ProjectDbContext _dbContext;
        private readonly IMediator mediator;

        public HomeController(IMediator mediator, ProjectDbContext dbContext)
        {
            this.mediator = mediator;
            _dbContext = dbContext;
        }
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal currentUser = User;
            var user = new ProjectUser();

            if (currentUser.Identity.IsAuthenticated)
            {
                string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userId, out int UserId);
                user = _dbContext.Users.Find(UserId);
            ViewBag.CurrentUserImage = !string.IsNullOrEmpty(user.ImagePath)
     ? user.ImagePath
     : "/images/Profile-Icon.png";
            }
            var role = _dbContext.UserRoles
                .Where(y => y.UserId == user.Id)
                .Select(r => r.RoleId).FirstOrDefault();

           


            ViewBag.professionName = _dbContext.Professions
       .Where(p => p.Id == user.ProfessionId)
       .Select(x => x.Name)
       .First();
            ViewBag.departmentName = _dbContext.Departments
       .Where(p => p.Id == user.DepartmentId)
       .Select(x => x.Name)
       .First();

            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Index(PermissionMultiModel model)
        {
            var currentUser = User;
            var user = new ProjectUser();
            if (currentUser.Identity.IsAuthenticated)
            {
                string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userId, out int UserId);
                user = _dbContext.Users.Find(UserId);
            }
            model.CreateCommand.ProjectUserId = user.Id;
            model.CreateCommand.Status = Status.Pending;
            var response = await mediator.Send(model.CreateCommand);

            // ✅ AJAX cavabı qaytar
            if (response == null)
                return Json(new { error = true, message = "Xəta baş verdi" });

            return Json(new { error = false, message = "Ərizəniz göndərildi" });
        }

        [Route("/about")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> About()
        {
            var currentUser = User;
            var user = new ProjectUser();

            if (currentUser.Identity.IsAuthenticated)
            {
                string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userId, out int UserId);
                user = _dbContext.Users.Find(UserId);
                ViewBag.CurrentUserImage = !string.IsNullOrEmpty(user.ImagePath)
     ? user.ImagePath
     : "/images/Profile-Icon.png";
            }

            var permissions = _dbContext.Permissions
                .Where(p => p.ProjectUserId == user.Id && p.DeletedDate==null)
                .ToList();
            var permissionMulti = new PermissionMultiModel
            {
                Permission = permissions,
                CreateCommand = new PermissionCreateCommand()
            };
            return View(permissionMulti);
        }
       
    }
}
