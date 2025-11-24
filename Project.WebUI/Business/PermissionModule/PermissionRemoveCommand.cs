using MediatR;
using Microsoft.EntityFrameworkCore;
using Project.WebUI.AppCode.Infrastructure;
using Project.WebUI.Models.DataContexts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.PermissionModule
{
    public class PermissionRemoveCommand : PageableModel, IRequest<JsonResponse>
    {
        public int Id { get; set; }
        public class PermissionRemoveCommandHandler : IRequestHandler<PermissionRemoveCommand, JsonResponse>
        {
            private readonly ProjectDbContext db;

            public PermissionRemoveCommandHandler(ProjectDbContext db)
            {
                this.db = db;
            }

            public async Task<JsonResponse> Handle(PermissionRemoveCommand request, CancellationToken cancellationToken)
            {
                var entity = await db.Permissions
               .FirstOrDefaultAsync(m => m.Id == request.Id , cancellationToken);


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
