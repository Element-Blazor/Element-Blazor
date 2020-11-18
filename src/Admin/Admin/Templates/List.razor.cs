using AutoMapper;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Element.Admin.Templates
{
    public partial class List<TModel, TViewModel> : BComponentBase
        where TModel : class
    {
        [Inject]
        private DbContext dbContext { get; set; }

        [Inject]
        private IMapper mapper { get; set; }

        private DbSet<TModel> dbset
        {
            get
            {
                return dbContext.Set<TModel>();
            }
        }
        private List<TViewModel> dataSources = new List<TViewModel>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var list = await dbset.ToListAsync();
            dataSources = list.Select(x => mapper.Map<TViewModel>(x)).ToList();
            await InvokeAsync(Refresh);
        }
    }
}
