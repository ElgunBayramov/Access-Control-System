using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
        public string Email { get; set; }        // ✅ YENİ
        public DateTime? Date { get; set; } = DateTime.Now;
        public string Duration { get; set; }
        public string Reason { get; set; }
        public Status Status { get; set; }
        public int ProjectUserId { get; set; }

        public class PermissionCreateCommandHandler
            : IRequestHandler<PermissionCreateCommand, Permission>
        {
            private readonly ProjectDbContext db;
            private readonly IActionContextAccessor ctx;

            public PermissionCreateCommandHandler(
                ProjectDbContext db,
                IActionContextAccessor ctx)
            {
                this.db = db;
                this.ctx = ctx;
            }

            public async Task<Permission> Handle(
                PermissionCreateCommand request,
                CancellationToken cancellationToken)
            {
                // Email boş olmamalıdır
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    ctx.ActionContext.ModelState
                        .AddModelError("Email", "Kənar adamın emaili daxil edilməlidir");
                    return null;
                }

                var permission = new Permission
                {
                    Name = request.Name,
                    Surname = request.Surname,
                    Profession = request.Profession,
                    Email = request.Email,          
                    Date = request.Date,
                    Duration = request.Duration,
                    Reason = request.Reason,
                    ProjectUserId = request.ProjectUserId,
                    Status = Status.Pending
                };

                await db.Permissions.AddAsync(permission, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
                return permission;
            }
        }
    }
}