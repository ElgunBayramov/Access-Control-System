using MediatR;
using Microsoft.EntityFrameworkCore;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.DepartmentModule
{
    public class DepartmentsAllQuery : IRequest<List<Department>>
    {
        public class ProjectsAllQueryHandler : IRequestHandler<DepartmentsAllQuery, List<Department>>
        {
            private readonly ProjectDbContext db;

            public ProjectsAllQueryHandler(ProjectDbContext db)
            {
                this.db = db;
            }
            public async Task<List<Department>> Handle(DepartmentsAllQuery request, CancellationToken cancellationToken)
            {
                var data = await db.Departments.Where(m => m.DeletedDate == null)
                    .ToListAsync(cancellationToken);

                return data;
            }
        }
    }
}
