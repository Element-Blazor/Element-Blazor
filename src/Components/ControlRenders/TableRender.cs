using Blazui.Component.ControlConfigs;
using Blazui.Component.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazui.Component.ControlRenders
{
    public class TableRender : ITableRender
    {
        private readonly MessageBox messageBox;
        BTable currentTable = null;

        public TableRender(MessageBox messageBox)
        {
            this.messageBox = messageBox;
        }
        public object Data { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Render(RenderTreeBuilder renderTreeBuilder, FormItemConfig config)
        {
            object tableDataSource = null;
            var attributeConfig = (TableAttribute)config.Config;
            renderTreeBuilder.OpenComponent(0, config.InputControl);
            renderTreeBuilder.AddAttribute(1, nameof(BFormItemObject.EnableAlwaysRender), true);
            renderTreeBuilder.AddAttribute(2, nameof(BTable.AutoGenerateColumns), true);
            renderTreeBuilder.AddAttribute(3, nameof(BTable.DataType), typeof(KeyValueModel));
            if (attributeConfig != null)
            {
                renderTreeBuilder.AddAttribute(4, nameof(BTable.Height), attributeConfig.Height);
                renderTreeBuilder.AddAttribute(5, nameof(BTable.IsEditable), attributeConfig.IsEditable);
                if (attributeConfig.IsEditable)
                {
                    renderTreeBuilder.AddAttribute(6, nameof(BTable.ChildContent), (RenderFragment)(builder1 =>
                    {
                        builder1.OpenComponent<BTableColumns>(7);
                        builder1.AddAttribute(8, nameof(BTableColumns.ChildContent), (RenderFragment)(builder2 =>
                        {
                            builder2.OpenComponent<BTableTemplateColumn>(9);
                            builder2.AddAttribute(10, nameof(BTableTemplateColumn.Text), "操作");
                            builder2.AddAttribute(11, nameof(BTableTemplateColumn.ChildContent), (RenderFragment<object>)(context => builder5 =>
                              {
                                  builder5.OpenComponent<BButton>(12);
                                  builder5.AddAttribute(13, nameof(BButton.Type), ButtonType.Danger);
                                  builder5.AddAttribute(14, nameof(BButton.IsCircle), true);
                                  builder5.AddAttribute(15, nameof(BButton.Icon), "el-icon-delete");
                                  builder5.AddAttribute(16, nameof(BButton.OnClick), EventCallback.Factory.Create<MouseEventArgs>(currentTable, e =>
                                   {
                                       _ = ExecuteDeleteAsync(context, tableDataSource);
                                   }));
                                  builder5.CloseComponent();
                              }));
                            builder2.CloseComponent();
                        }));
                        builder1.CloseComponent();
                    }));
                }
            }
            renderTreeBuilder.AddAttribute(17, nameof(BTable.DataSourceChanged), EventCallback.Factory.Create<object>(this, dataSource =>
             {
                 tableDataSource = dataSource;
             }));
            renderTreeBuilder.AddAttribute(18, nameof(BTable.DataSource), tableDataSource);
            renderTreeBuilder.AddComponentReferenceCapture(19, table => currentTable = (BTable)table);
            renderTreeBuilder.CloseComponent();
        }

        private async Task ExecuteDeleteAsync(object context, object tableDataSource)
        {
            var result = await messageBox.ConfirmAsync("确认删除吗？");
            if (result != MessageBoxResult.Ok)
            {
                return;
            }
            ((List<KeyValueModel>)tableDataSource).Remove((KeyValueModel)context);
            Console.WriteLine("Remove Success");
            currentTable.MarkAsRequireRender();
            currentTable.Refresh();
        }
    }
}
