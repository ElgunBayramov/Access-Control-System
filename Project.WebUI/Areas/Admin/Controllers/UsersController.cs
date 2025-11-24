using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.WebUI.AppCode.Extensions;
using Project.WebUI.Business.UserModule;
using Project.WebUI.Models.DataContexts;
using System.Linq;
using System.Threading.Tasks;

namespace Project.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        private readonly IMediator mediator;
        private readonly ProjectDbContext db;

        public UsersController(IMediator mediator, ProjectDbContext db)
        {
            this.mediator = mediator;
            this.db = db;
        }

        public async Task<IActionResult> Index(UsersPagedQuery query)
        {
            var response = await mediator.Send(query);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListBody", response);
            }

            return View(response);
        }

        public async Task<IActionResult> Details(UserDetailQuery query)
        {
            ViewBag.AvailableRoles = await mediator.Send(new UserAvailableRolesQuery() { UserId = query.Id });
            ViewBag.AvailablePrincipals = await mediator.Send(new UserAvailablePrincipalsQuery() { UserId = query.Id });

            var data = await mediator.Send(query);
            if(data.ProfessionId != null || data.DepartmentId != null) {
                ViewBag.professionName = db.Professions
         .Where(p => p.Id == data.ProfessionId)
         .Select(x => x.Name)
         .First();
                ViewBag.departmentName = db.Departments
           .Where(p => p.Id == data.DepartmentId)
           .Select(x => x.Name)
           .First();
            }
            return View(data);
        }

        [HttpPost]
        [Authorize(Policy = "admin.users.setrole")]
        public async Task<IActionResult> SetRole(UserSetRoleCommand command)
        {
            var response = await mediator.Send(command);

            return Json(response);
        }

        [HttpPost]
        [Authorize(Policy = "admin.users.setprincipal")]
        public async Task<IActionResult> SetPrincipal(UserSetPrincipalCommand command)
        {
            var response = await mediator.Send(command);

            return Json(response);
        }
    }
}
