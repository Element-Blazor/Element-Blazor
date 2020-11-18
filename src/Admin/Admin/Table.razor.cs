using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace Element.Admin
{
    public partial class Table : BComponentBase
    {

        [Inject]
        private DbContext dbContext { get; set; }
        private Dictionary<string, IEntityType> tables = new Dictionary<string, IEntityType>();
        private List<TableFieldGen> fields = new List<TableFieldGen>();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            tables = dbContext.Model.GetEntityTypes().ToDictionary(x => x.Name);
        }

        private void LoadTableStructure(IEntityType table)
        {
            fields = table.GetDeclaredProperties().Select(x => new TableFieldGen()
            {
                Enable = true,
                EnableCreate = !x.IsPrimaryKey(),
                EnableUpdate = !x.IsPrimaryKey(),
                EnableList = true,
                Name = x.Name
            }).ToList();
            Refresh();
        }
    }
}
