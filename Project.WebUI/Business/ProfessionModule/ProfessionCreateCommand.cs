using MediatR;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.ProfessionModule
{
    public class ProfessionCreateCommand : IRequest<Profession>
    {
        public string Name { get; set; }
        public class ProfessionCreateCommandHandler : IRequestHandler<ProfessionCreateCommand, Profession>
        {
            private readonly ProjectDbContext db;

            public ProfessionCreateCommandHandler(ProjectDbContext db)
            {
                this.db = db;
            }
            public async Task<Profession> Handle(ProfessionCreateCommand request, CancellationToken cancellationToken)
            {
                var profession = new Profession()
                {
                    Name = request.Name,
                };

                await db.Professions.AddAsync(profession, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
                return profession;
            }
        }
    }
}
