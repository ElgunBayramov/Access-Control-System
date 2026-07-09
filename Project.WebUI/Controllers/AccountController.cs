using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.WebUI.Business.AccountModule;
using Project.WebUI.Models.Entities.Membership;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMediator mediator;
        private readonly UserManager<ProjectUser> userManager;

        public AccountController(IMediator mediator, UserManager<ProjectUser> userManager)
        {
            this.mediator = mediator;
            this.userManager = userManager;
        }

        [Route("/signin.html")]
        public IActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        [Route("/signin.html")]
        public async Task<IActionResult> Signin(SigninCommand command)
        {
            var user = await mediator.Send(command);

            if (!ModelState.IsValid || user == null)
            {
                return View(command);
            }

            var roles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var props = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

            var callbackUrl = Request.Query["ReturnUrl"].ToString();


            if (roles.Contains("admin"))
            {
                return Redirect("/admin");
            }
            if (roles.Contains("muhafize"))
            {
                return Redirect("/guard/watch");  
            }

            if (!string.IsNullOrWhiteSpace(callbackUrl))
            {
                return Redirect(callbackUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [Route("/accessdenied.html")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Route("/signout.html")]
        public async Task<IActionResult> Signout(SignoutCommand command)
        {
            await mediator.Send(command);
//var callback = Request.Headers["Referer"];

//if (!string.IsNullOrWhiteSpace(callback))
//{
//    return Redirect(callback);
//}
            return RedirectToAction("Index", "Home");
        }
    }
}

