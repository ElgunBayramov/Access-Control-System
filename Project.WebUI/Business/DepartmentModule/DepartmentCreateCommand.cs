using MediatR;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.DepartmentModule
{
    public class DepartmentCreateCommand : IRequest<Department>
    {
        public string Name { get; set; }
        public class DepartmentCreateCommandHandler : IRequestHandler<DepartmentCreateCommand, Department>
        {
            private readonly ProjectDbContext db;

            public DepartmentCreateCommandHandler(ProjectDbContext db)
            {
                this.db = db;
            }
            public async Task<Department> Handle(DepartmentCreateCommand request, CancellationToken cancellationToken)
            {
                var department = new Department()
                {
                    Name = request.Name,
                };

                await db.Departments.AddAsync(department, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
                return department;
            }
        }
    }
}
