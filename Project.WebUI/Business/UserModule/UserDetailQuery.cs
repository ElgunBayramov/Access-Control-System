using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.WebUI.Models.Entities.Membership;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.UserModule
{
    public class UserDetailQuery : IRequest<ProjectUser>
    {
        public int Id { get; set; }


        public class UserDetailQueryHandler : IRequestHandler<UserDetailQuery, ProjectUser>
        {
            private readonly UserManager<ProjectUser> userManager;

            public UserDetailQueryHandler(UserManager<ProjectUser> userManager)
            {
                this.userManager = userManager;
            }

            public async Task<ProjectUser> Handle(UserDetailQuery request, CancellationToken cancellationToken)
            {
                var data = await userManager.Users.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);


                return data;
            }
        }
    }
}
