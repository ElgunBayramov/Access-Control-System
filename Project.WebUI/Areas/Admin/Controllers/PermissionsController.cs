using MediatR;
using Microsoft.AspNetCore.Mvc;
using Project.WebUI.AppCode.Extensions;
using Project.WebUI.Business.PermissionModule;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Project.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PermissionsController : Controller
    {
        private readonly IMediator mediator;
        private readonly ProjectDbContext dbContext;
        public PermissionsController(IMediator mediator, ProjectDbContext dbContext)
        {
            this.mediator = mediator;
            this.dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> Index(PermissionPagedQuery query)
        {
            var response = await mediator.Send(query);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListBody", response);
            }

            return View(response);
        }
        public async Task<IActionResult> Details(PermissionSingleQuery query)
        {
            var response = await mediator.Send(query);

            if (response == null)
            {
                return NotFound();
            }

            return View(response);
        }


        [HttpPost]
        public IActionResult UpdateStatus(int UserId, string AdditionalParam)
        {
            var user = dbContext.Permissions.FirstOrDefault(u => u.Id == UserId);

            if (user != null)
            {
                switch (AdditionalParam)
                {
                    case "Accept":
                        {
                            user.Status = Status.İcazəVerildi;
                            break;
                        }
                    case "Refuse":
                        {
                            user.Status = Status.İcazəVerilməyib;
                            break;
                        }
                    default:
                        break;
                }

                dbContext.SaveChanges();
                return Ok("Status updated successfully");
            }
            else
            {
                return NotFound("User not found");
            }
        }


        //[HttpPost]
        //public async Task<IActionResult> Remove(PermissionRemoveCommand command)
        //{
        //    var response = await mediator.Send(command);

        //    if (response.Error)
        //    {
        //        return Json(response);
        //    }
        //    var newQuery = new PermissionPagedQuery
        //    {
        //        PageIndex = command.PageIndex,
        //        PageSize = command.PageSize
        //    };
        //    var data = await mediator.Send(newQuery);
        //    return PartialView("_ListBody", data);
        //}
    }
}
