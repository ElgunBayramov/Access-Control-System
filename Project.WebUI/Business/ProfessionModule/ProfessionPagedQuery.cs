using MediatR;
using Project.WebUI.AppCode.Infrastructure;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities;
using Project.WebUI.Models.ViewModels;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.ProfessionModule
{
    public class ProfessionPagedQuery : PageableModel, IRequest<PagedViewModel<Profession>>
    {
        public override int PageSize
        {
            get
            {
                if (base.PageSize < 6)
                    base.PageSize = 6;

                return base.PageSize;
            }
        }

        public class ProfessionPagedQueryHandler : IRequestHandler<ProfessionPagedQuery, PagedViewModel<Profession>>
        {
            private readonly ProjectDbContext db;

            public ProfessionPagedQueryHandler(ProjectDbContext db)
            {
                this.db = db;
            }

            public async Task<PagedViewModel<Profession>> Handle(ProfessionPagedQuery request, CancellationToken cancellationToken)
            {
                var query = db.Professions
               .Where(m => m.DeletedDate == null)
                 .AsQueryable();

                query = query.OrderByDescending(m => m.Id);

                var data = new PagedViewModel<Profession>(query, request.PageIndex, request.PageSize);

                return data;
            }
        }
    }
}
