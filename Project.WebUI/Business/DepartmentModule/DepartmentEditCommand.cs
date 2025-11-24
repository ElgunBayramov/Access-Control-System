using MediatR;
using Microsoft.EntityFrameworkCore;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.DepartmentModule
{
    public class DepartmentEditCommand : IRequest<Department>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public class DepartmentEditCommandHandler : IRequestHandler<DepartmentEditCommand, Department>
        {
            private readonly ProjectDbContext db;

            public DepartmentEditCommandHandler(ProjectDbContext db)
            {
                this.db = db;
            }
            public async Task<Department> Handle(DepartmentEditCommand request, CancellationToken cancellationToken)
            {
                var department = await db.Departments.FirstOrDefaultAsync(m => m.Id == request.Id && m.DeletedDate == null, cancellationToken);
                if (department == null)
                {
                    return null;
                }
                department.Name = request.Name;
                await db.SaveChangesAsync(cancellationToken);
                return department;
            }
        }
    }
}
