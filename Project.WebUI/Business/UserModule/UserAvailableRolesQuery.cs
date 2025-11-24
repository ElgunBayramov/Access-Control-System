using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.WebUI.Models.Dtos.Roles;
using Project.WebUI.Models.Entities.Membership;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.UserModule
{
    public class UserAvailableRolesQuery : IRequest<IEnumerable<AvailableRole>>
    {
        public int UserId { get; set; }


        public class UserAvailableRolesQueryHandler : IRequestHandler<UserAvailableRolesQuery, IEnumerable<AvailableRole>>
        {
            private readonly UserManager<ProjectUser> userManager;
            private readonly RoleManager<ProjectRole> roleManager;

            public UserAvailableRolesQueryHandler(UserManager<ProjectUser> userManager, RoleManager<ProjectRole> roleManager)
            {
                this.userManager = userManager;
                this.roleManager = roleManager;
            }
            public async Task<IEnumerable<AvailableRole>> Handle(UserAvailableRolesQuery request, CancellationToken cancellationToken)
            {
                var user = await userManager.Users.FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

                var userRoles = await userManager.GetRolesAsync(user);

                var roles = (await roleManager.Roles.ToListAsync(cancellationToken))
                            .Select(m => new AvailableRole
                            {
                                RoleName = m.Name,
                                Selected = userRoles.Contains(m.Name)
                            });


                return roles;
            }
        }
    }





}
