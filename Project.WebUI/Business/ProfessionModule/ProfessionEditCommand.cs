using MediatR;
using Microsoft.EntityFrameworkCore;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.ProfessionModule
{
    public class ProfessionEditCommand : IRequest<Profession>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public class ProfessionEditCommandHandler : IRequestHandler<ProfessionEditCommand, Profession>
        {
            private readonly ProjectDbContext db;

            public ProfessionEditCommandHandler(ProjectDbContext db)
            {
                this.db = db;
            }
            public async Task<Profession> Handle(ProfessionEditCommand request, CancellationToken cancellationToken)
            {
                var Profession = await db.Professions.FirstOrDefaultAsync(m => m.Id == request.Id && m.DeletedDate == null, cancellationToken);
                if(Profession == null)
                {
                    return null;
                }
                Profession.Name = request.Name;
                await db.SaveChangesAsync(cancellationToken);
                return Profession;   
            }
        }
    }
}
