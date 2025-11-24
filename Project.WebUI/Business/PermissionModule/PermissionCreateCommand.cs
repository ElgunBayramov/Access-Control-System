using MediatR;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.PermissionModule
{
    public class PermissionCreateCommand : IRequest<Permission>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Profession { get; set; }
        public DateTime? Date { get; set; } = DateTime.Now;
        public string Duration { get; set; }
        public string Reason { get; set; }
        public Status Status { get; set; }
        public int ProjectUserId { get; set; }
        public class PermissionCreateCommandHandler : IRequestHandler<PermissionCreateCommand, Permission>
        {
            private readonly ProjectDbContext db;

            public PermissionCreateCommandHandler(ProjectDbContext db)

            {
                this.db = db;
            }
            public async Task<Permission> Handle(PermissionCreateCommand request, CancellationToken cancellationToken)
            {
                var permission = new Permission()
                {
                    Name=request.Name,
                    Surname = request.Surname,
                    Profession = request.Profession,
                    Date = request.Date,
                    Duration = request.Duration,
                    Reason = request.Reason,
                    ProjectUserId = request.ProjectUserId,
                };

                await db.Permissions.AddAsync(permission, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
                return permission;
            }
        }
    }
}
