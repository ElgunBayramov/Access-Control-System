using MediatR;
using Microsoft.EntityFrameworkCore;
using Project.WebUI.AppCode.Infrastructure;
using Project.WebUI.Models.DataContexts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.DepartmentModule
{
    public class DepartmentRemoveCommand : PageableModel, IRequest<JsonResponse>
    {
        public int Id { get; set; }
        public class DepartmentRemoveCommandHandler : IRequestHandler<DepartmentRemoveCommand, JsonResponse>
        {
            private readonly ProjectDbContext db;

            public DepartmentRemoveCommandHandler(ProjectDbContext db)
            {
                this.db = db;
            }

            public async Task<JsonResponse> Handle(DepartmentRemoveCommand request, CancellationToken cancellationToken)
            {
                var entity = await db.Departments
               .FirstOrDefaultAsync(m => m.Id == request.Id && m.DeletedDate == null, cancellationToken);


                if (entity == null)
                {
                    return new JsonResponse
                    {
                        Error = true,
                        Message = "Qeyd tapilmadi"
                    };
                }

                entity.DeletedDate = DateTime.UtcNow.AddHours(4);
                await db.SaveChangesAsync(cancellationToken);

                return new JsonResponse
                {
                    Error = false,
                    Message = "Ugurludur"
                };
            }
        }
    }
}
