using MediatR;
using Microsoft.EntityFrameworkCore;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.PermissionModule
{
    public class PermissionsAllQuery : IRequest<List<Permission>>
    {
        public class ProjectsAllQueryHandler : IRequestHandler<PermissionsAllQuery, List<Permission>>
        {
            private readonly ProjectDbContext db;

            public ProjectsAllQueryHandler(ProjectDbContext db)
            {
                this.db = db;
            }
            public async Task<List<Permission>> Handle(PermissionsAllQuery request, CancellationToken cancellationToken)
            {
                var data = await db.Permissions
                   .Where(m => m.DeletedDate == null)
                    .ToListAsync(cancellationToken);

                return data;
            }
        }
    }
}
