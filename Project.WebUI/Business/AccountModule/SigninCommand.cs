using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Project.WebUI.AppCode.Extensions;
using Project.WebUI.Models.Entities.Membership;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.AccountModule
{
    public class SigninCommand : IRequest<ProjectUser>
    {
        public string UserName { get; set; }
        public string Password { get; set; }


        public class SigninCommandHandler : IRequestHandler<SigninCommand, ProjectUser>
        {
            private readonly SignInManager<ProjectUser> signinManager;
            private readonly IActionContextAccessor ctx;

            public SigninCommandHandler(SignInManager<ProjectUser> signinManager, IActionContextAccessor ctx)
            {
                this.signinManager = signinManager;
                this.ctx = ctx;
            }


            public async Task<ProjectUser> Handle(SigninCommand request, CancellationToken cancellationToken)
            {
                ProjectUser user = null;


                if (request.UserName.IsEmail())
                {
                    user = await signinManager.UserManager.FindByEmailAsync(request.UserName);
                }
                else
                {
                    user = await signinManager.UserManager.FindByNameAsync(request.UserName);
                }


                if (user == null)
                {
                    ctx.ActionContext.ModelState.AddModelError("UserName", "Istifadəçi adı və ya şifrə səhvdir");

                    return null;
                }

                var result = await signinManager.CheckPasswordSignInAsync(user, request.Password, true);


                if (result.IsLockedOut)
                {
                    ctx.ActionContext.ModelState.AddModelError("UserName", "Hesabınız keçici olaraq məhdudlaşdırılıb");

                    return null;
                }


                if (result.IsNotAllowed)
                {
                    ctx.ActionContext.ModelState.AddModelError("UserName", "Hesaba daxil olmaq mümkün deyil");

                    return null;
                }



                if (!user.EmailConfirmed)
                {
                    ctx.ActionContext.ModelState.AddModelError("UserName", "Hesabınız təsdiq edilməyib");

                    return null;
                }


                if (result.Succeeded)
                {
                    return user;
                }

                ctx.ActionContext.ModelState.AddModelError("UserName", "Istifadəçi adı və ya şifrə səhvdir");
                return null;
            }
        }
    }
}
