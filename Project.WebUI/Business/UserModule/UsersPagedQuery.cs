using MediatR;
using Microsoft.AspNetCore.Identity;
using Project.WebUI.AppCode.Infrastructure;
using Project.WebUI.Models.Entities.Membership;
using Project.WebUI.Models.ViewModels;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.UserModule
{
    public class UsersPagedQuery : PageableModel, IRequest<PagedViewModel<ProjectUser>>
    {
        public override int PageSize
        {
            get
            {
                if (base.PageSize < 6)
                    base.PageSize = 6;

                return base.PageSize;
            }
        }


        public class UsersPagedQueryHandler : IRequestHandler<UsersPagedQuery, PagedViewModel<ProjectUser>>
        {
            private readonly UserManager<ProjectUser> userManager;

            public UsersPagedQueryHandler(UserManager<ProjectUser> userManager)
            {
                this.userManager = userManager;
            }

            public Task<PagedViewModel<ProjectUser>> Handle(UsersPagedQuery request, CancellationToken cancellationToken)
            {
                var query = userManager.Users
                    .OrderBy(m => m.EmailConfirmed)
                    .ThenByDescending(m => m.Id);


                var pagedData = new PagedViewModel<ProjectUser>(query, request.PageIndex, request.PageSize);

                return Task.FromResult(pagedData);
            }
        }
    }
}
