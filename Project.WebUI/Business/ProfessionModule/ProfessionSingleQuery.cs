using MediatR;
using Microsoft.EntityFrameworkCore;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.ProfessionModule
{
    public class ProfessionSingleQuery : IRequest<Profession>
    {
        public int Id { get; set; }
        public class ProfessionSingleQueryHandler : IRequestHandler<ProfessionSingleQuery, Profession>
        {
            private readonly ProjectDbContext db;

            public ProfessionSingleQueryHandler(ProjectDbContext db)
            {
                this.db = db;
            }
            public async Task<Profession> Handle(ProfessionSingleQuery request, CancellationToken cancellationToken)
            {
                var entity = await db.Professions.FirstOrDefaultAsync(m => m.Id == request.Id && m.DeletedDate == null, cancellationToken);
                return entity;
            }
        }
    }
}
