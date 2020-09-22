using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element.Markdown.IconHandlers
{
    public class TableHandler : IIconHandler
    {
        private readonly IJSRuntime jSRuntime;
        private readonly DialogService dialogService;

        public TableHandler(IJSRuntime jSRuntime, DialogService dialogService)
        {
            this.jSRuntime = jSRuntime;
            this.dialogService = dialogService;
        }

        public async Task HandleAsync(BMarkdownEditor editor)
        {
            var model = await dialogService.ShowDialogAsync<CreateTable, CreateTableModel>("插入表格", 400);
            if(model.Result!=null&&model.Result.Columns>0)
            {
                var headers = Enumerable.Range(1, model.Result.Columns).Select(x => "   列" + x + "   ").ToArray();
                var lines = Enumerable.Range(1, model.Result.Columns).Select(x => " :-------").ToArray();
                var columns = Enumerable.Range(1, model.Result.Columns).Select(x => "         ").ToArray();
                var rows = new List<string>();
                rows.Add("|" + string.Join("|", headers) + "|");
                rows.Add("|" + string.Join("|", lines) + "|");
                rows.Add("|" + string.Join("|", columns) + "|");
                await jSRuntime.InvokeVoidAsync("append", editor.Textarea, string.Join(Environment.NewLine, rows));
            }
        }
    }
}
