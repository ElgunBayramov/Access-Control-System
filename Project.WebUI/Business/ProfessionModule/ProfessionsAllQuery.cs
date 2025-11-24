using MediatR;
using Microsoft.EntityFrameworkCore;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.ProfessionModule
{
    public class ProfessionsAllQuery : IRequest<List<Profession>>
    {
        public class ProjectsAllQueryHandler : IRequestHandler<ProfessionsAllQuery, List<Profession>>
        {
            private readonly ProjectDbContext db;

            public ProjectsAllQueryHandler(ProjectDbContext db)
            {
                this.db = db;
            }
            public async Task<List<Profession>> Handle(ProfessionsAllQuery request, CancellationToken cancellationToken)
            {
                var data = await db.Professions.Where(m => m.DeletedDate == null)
                    .ToListAsync(cancellationToken);

                return data;
            }
        }
    }
}
