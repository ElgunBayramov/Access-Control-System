using MediatR;
using Microsoft.AspNetCore.Mvc;
using Project.WebUI.AppCode.Extensions;
using Project.WebUI.Business.ProfessionModule;
using System.Threading.Tasks;

namespace Project.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProfessionsController : Controller
    {
        private readonly IMediator mediator;

        public ProfessionsController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        public async Task<IActionResult> Index(ProfessionPagedQuery query)
        {
            var response = await mediator.Send(query);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListBody", response);
            }

            return View(response);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProfessionCreateCommand command)
        {
            var response = await mediator.Send(command);
            if (response == null)
            {
                return View(command);
            }

            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Details(ProfessionSingleQuery query)
        {
            var response = await mediator.Send(query);

            if (response == null)
            {
                return NotFound();
            }

            return View(response);
        }

        public async Task<IActionResult> Edit(ProfessionSingleQuery query)
        {
            var response = await mediator.Send(query);
            if (response == null)
            {
                return NotFound();
            }
            var command = new ProfessionEditCommand();
            command.Name = response.Name;
            return View(command);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfessionEditCommand command)
        {
            var response = await mediator.Send(command);
            if (response == null)
            {
                return View(command);
            }

            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        public async Task<IActionResult> Remove(ProfessionRemoveCommand command)
        {
            var response = await mediator.Send(command);


            if (response.Error)
            {
                return Json(response);
            }
            var newQuery = new ProfessionPagedQuery
            {
                PageIndex = command.PageIndex,
                PageSize = command.PageSize
            };
            var data = await mediator.Send(newQuery);
            return PartialView("_ListBody", data);
        }
    }
}
