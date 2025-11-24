using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project.WebUI.Business.AccountModule;
using Project.WebUI.Business.DepartmentModule;
using Project.WebUI.Business.ProfessionModule;
using System;
using System.Threading.Tasks;

namespace Project.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly IMediator mediator;

        public AccountController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<IActionResult> Register()
        {
            var professions = await mediator.Send(new ProfessionsAllQuery());
            ViewBag.ProfessionId = new SelectList(professions, "Id", "Name");
            var departments = await mediator.Send(new DepartmentsAllQuery());
            ViewBag.DepartmentId = new SelectList(departments, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterCommand command)
        {
            var response = await mediator.Send(command);

            if (!ModelState.IsValid)
            {
                return View(command);
            }
            var professions = await mediator.Send(new ProfessionsAllQuery());
            ViewBag.ProfessionId = new SelectList(professions, "Id", "Name");
            var departments = await mediator.Send(new DepartmentsAllQuery());
            ViewBag.DepartmentId = new SelectList(departments, "Id", "Name");

            TempData["Message"] = $"{command.Email} - E-poct`a gonderilen linkle qeydiyyati tamamlayin";
            return RedirectToAction("Index", "Users");
        }


        [Route("/email-confirm")]
        public async Task<IActionResult> EmailConfirmation(RegisterConfirmationCommand command)
        {
            var response = await mediator.Send(command);

            if (!ModelState.IsValid)
            {
                return View(command);
            }
            return RedirectToAction(nameof(SignIn));
        }
    }
}
