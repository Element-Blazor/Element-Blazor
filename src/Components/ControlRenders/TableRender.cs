using Element.ControlConfigs;
using Element.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element.ControlRenders
{
    public class TableRender : ITableRender
    {
        public static async Task DefaultSaverAsync(TableSaveEventArgs arg)
        {
            try
            {
                if (arg.Table.DataSource is List<KeyValueModel> keyValues)
                {
                    switch (arg.Action)
                    {
                        case SaveAction.Create:
                            keyValues.Add((KeyValueModel)arg.Data);
                            break;
                        case SaveAction.Delete:
                            keyValues.Remove((KeyValueModel)arg.Data);
                            break;
                        case SaveAction.Update:
                            var data = keyValues.FirstOrDefault(x => x.Key == ((KeyValueModel)arg.Data).Key);
                            if (data == null)
                            {
                                keyValues.Add((KeyValueModel)arg.Data);
                                return;
                            }
                            data.Value = ((KeyValueModel)arg.Data).Value;
                            break;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(arg.Key))
                {
                    Console.WriteLine(arg.Key);
                    var list = (arg.Table.DataSource as IEnumerable).Cast<object>().ToList();
                    var propertyKey = arg.DataType.GetProperty(arg.Key);
                    switch (arg.Action)
                    {
                        case SaveAction.Create:
                            list.Add(arg.Data);
                            break;
                        case SaveAction.Delete:
                            list.Remove(arg.Data);
                            break;
                        case SaveAction.Update:
                            var currentKey = propertyKey.GetValue(arg.Data);
                            var data = list.FirstOrDefault(x => propertyKey.GetValue(x) == currentKey);
                            if (data == null)
                            {
                                list.Add(arg.Data);
                                return;
                            }
                            var properties = arg.DataType.GetProperties();
                            foreach (var property in properties)
                            {
                                property.SetValue(data, property.GetValue(arg.Data));
                            }
                            break;
                    }
                    arg.Table.DataSource = list;
                }
                if (arg.Table.DataSourceChanged.HasDelegate)
                {
                    await arg.Table.DataSourceChanged.InvokeAsync(arg.Table.DataSource);
                }
                arg.Table.Refresh();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private readonly MessageService message;
        private readonly MessageBox messageBox;
        ElTable currentTable = null;

        public TableRender(MessageService message, MessageBox messageBox)
        {
            this.message = message;
            this.messageBox = messageBox;
        }

        public void Render(RenderTreeBuilder renderTreeBuilder, RenderConfig config)
        {
            List<KeyValueModel> tableDataSource = new List<KeyValueModel>();
            var attributeConfig = (TableAttribute)config.ControlAttribute;
            renderTreeBuilder.OpenComponent(0, config.InputControlType);
            renderTreeBuilder.AddAttribute(1, nameof(ElFormItemObject.EnableAlwaysRender), true);
            renderTreeBuilder.AddAttribute(2, nameof(ElTable.AutoGenerateColumns), true);
            renderTreeBuilder.AddAttribute(3, nameof(ElTable.DataType), typeof(KeyValueModel));
            if (attributeConfig != null)
            {
                renderTreeBuilder.AddAttribute(4, nameof(ElTable.Height), attributeConfig.Height);
                renderTreeBuilder.AddAttribute(5, nameof(ElTable.IsEditable), attributeConfig.IsEditable);
                if (attributeConfig.IsEditable)
                {
                    renderTreeBuilder.AddAttribute(6, nameof(ElTable.DataType), typeof(KeyValueModel));
                    if (config.Page == null)
                    {
                        ExceptionHelper.Throw(ExceptionHelper.CascadingValueNotFound, "表格启用可编辑功能后必须在外面套一层 CascadingValue，值为 this，名称为 Page");
                    }
                    renderTreeBuilder.AddAttribute(7, nameof(ElTable.OnSave), EventCallback.Factory.Create<TableSaveEventArgs>(config.Page, DefaultSaverAsync));
                }
            }
            renderTreeBuilder.AddAttribute(8, nameof(ElTable.DataSourceChanged), EventCallback.Factory.Create<object>(this, dataSource =>
             {
                 tableDataSource = (List<KeyValueModel>)dataSource;
                 config.EditingValue = dataSource;
             }));
            renderTreeBuilder.AddAttribute(9, nameof(ElTable.DataSource), tableDataSource);
            renderTreeBuilder.AddComponentReferenceCapture(10, table => currentTable = (ElTable)table);
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
            currentTable.Refresh();
        }
    }
}
