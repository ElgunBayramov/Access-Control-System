using MediatR;
using Project.WebUI.AppCode.Infrastructure;
using Project.WebUI.Models.DataContexts;
using Project.WebUI.Models.Entities;
using Project.WebUI.Models.ViewModels;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Project.WebUI.Business.DepartmentModule
{
    public class DepartmentPagedQuery : PageableModel, IRequest<PagedViewModel<Department>>
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

        public class DepartmentPagedQueryHandler : IRequestHandler<DepartmentPagedQuery, PagedViewModel<Department>>
        {
            private readonly ProjectDbContext db;

            public DepartmentPagedQueryHandler(ProjectDbContext db)
            {
                this.db = db;
            }

            public async Task<PagedViewModel<Department>> Handle(DepartmentPagedQuery request, CancellationToken cancellationToken)
            {
                var query = db.Departments
               .Where(m => m.DeletedDate == null)
                 .AsQueryable();

                query = query.OrderByDescending(m => m.Id);

                var data = new PagedViewModel<Department>(query, request.PageIndex, request.PageSize);

                return data;
            }
        }
    }
}
