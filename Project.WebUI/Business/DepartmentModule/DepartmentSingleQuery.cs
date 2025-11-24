using MediatR;
using Microsoft.EntityFrameworkCore;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.DepartmentModule
{
    public class DepartmentSingleQuery : IRequest<Department>
    {
        public int Id { get; set; }
        public class DepartmentSingleQueryHandler : IRequestHandler<DepartmentSingleQuery, Department>
        {
            private readonly ProjectDbContext db;

            public DepartmentSingleQueryHandler(ProjectDbContext db)
            {
                this.db = db;
            }
            public async Task<Department> Handle(DepartmentSingleQuery request, CancellationToken cancellationToken)
            {
                var entity = await db.Departments.FirstOrDefaultAsync(m => m.Id == request.Id && m.DeletedDate == null, cancellationToken);
                return entity;
            }
        }
    }
}
