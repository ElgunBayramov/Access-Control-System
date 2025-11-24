using MediatR;
using Project.WebUI.AppCode.Infrastructure;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities;
using Project.WebUI.Models.ViewModels;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.PermissionModule
{
    public class PermissionPagedQuery : PageableModel, IRequest<PagedViewModel<Permission>>
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

        public class PermissionPagedQueryHandler : IRequestHandler<PermissionPagedQuery, PagedViewModel<Permission>>
        {
            private readonly ProjectDbContext db;

            public PermissionPagedQueryHandler(ProjectDbContext db)
            {
                this.db = db;
            }

            public async Task<PagedViewModel<Permission>> Handle(PermissionPagedQuery request, CancellationToken cancellationToken)
            {
                var query = db.Permissions
                 .Where(m => m.DeletedDate == null && m.Status == Status.Gözlənilir)
                 .AsQueryable();

                query = query.OrderByDescending(m => m.Id);

                var data = new PagedViewModel<Permission>(query, request.PageIndex, request.PageSize);

                return data;
            }
        }
    }
}
