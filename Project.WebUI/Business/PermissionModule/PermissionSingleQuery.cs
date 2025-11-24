using MediatR;
using Microsoft.EntityFrameworkCore;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.PermissionModule
{
    public class PermissionSingleQuery : IRequest<Permission>
    {
        public int Id { get; set; }
        public class PermissionSingleQueryHandler : IRequestHandler<PermissionSingleQuery, Permission>
        {
            private readonly ProjectDbContext db;

            public PermissionSingleQueryHandler(ProjectDbContext db)
            {
                this.db = db;
            }
            public async Task<Permission> Handle(PermissionSingleQuery request, CancellationToken cancellationToken)
            {
                var entity = await db.Permissions.FirstOrDefaultAsync(m => m.Id == request.Id && m.DeletedDate == null, cancellationToken);
                return entity;
            }
            public async void Accept(int UserId)
            {
                var user =  db.Permissions
                    .First();

                user.Status = Status.İcazəVerildi;
                db.SaveChanges();
            }
        }
    }
}
