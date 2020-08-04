



using Blazui.Component.ControlConfigs;
using Blazui.Component.ControlRender;
using Blazui.Component.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public partial class BTable : IContainerComponent
    {
        private SaveAction saveAction = SaveAction.Create;
        private TaskCompletionSource<int> editingTaskCompletionSource;
        private bool dataSourceUpdated = false;
        private BButton saveAddButton;
        private bool renderCompleted = false;
        private ElementReference editingRowEl;
        internal BCheckBox<bool> chkAll;
        internal ElementReference headerElement;
        private object editingRow;
        internal List<TableHeader> Headers { get; set; } = new List<TableHeader>();
        internal bool headerInitilized = false;
        internal bool headerRendered = false;
        private IDictionary<BCheckBox<bool>, object> rowCheckBoxses = new Dictionary<BCheckBox<bool>, object>();
        internal int headerHeight = 49;
        internal List<object> rows = new List<object>();
        internal List<object> hiddenRows = new List<object>();

        [Inject]
        private TableEditorMap map { get; set; }

        [Inject]
        private IServiceProvider provider { get; set; }

        [Inject]
        private HttpClient httpClient { get; set; }

        /// <summary>
        /// 当保存数据时触发
        /// </summary>
        [Parameter]
        public EventCallback<TableSaveEventArgs> OnSave { get; set; }

        /// <summary>
        /// 表格可编辑
        /// </summary>
        [Parameter]
        public bool IsEditable { get; set; }

        /// <summary>
        /// 忽略的字段名称
        /// </summary>
        [Parameter]
        public string[] IgnoreProperties { get; set; } = { };

        /// <summary>
        /// 数据获取地址
        /// </summary>
        [Parameter]
        public string Url { get; set; }

        /// <summary>
        /// 数据源
        /// </summary>
        [Parameter]
        public object DataSource { get; set; }

        /// <summary>
        /// 当数据源发生变化时触发
        /// </summary>
        [Parameter]
        public EventCallback<object> DataSourceChanged { get; set; }
        /// <summary>
        /// 动态加载数据时设置实体类型
        /// </summary>
        [Parameter]
        public Type DataType { get; set; }

        /// <summary>
        /// 是否自动生成列
        /// </summary>
        [Parameter]
        public bool AutoGenerateColumns { get; set; } = true;

        /// <summary>
        /// 是否在第一列显示复选框列
        /// </summary>
        [Parameter]
        public bool HasSelectionColumn { get; set; } = true;

        /// <summary>
        /// 当表格无数据时显示的消息
        /// </summary>
        [Parameter]
        public string EmptyMessage { get; set; }
        /// <summary>
        /// 总数据条数
        /// </summary>
        [Parameter]
        public int Total { get; set; }

        /// <summary>
        /// 每页条数
        /// </summary>
        [Parameter]
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 当前页数
        /// </summary>
        [Parameter]
        public int CurrentPage
        {
            get
            {
                return currentPage;
            }
            set
            {
                currentPage = value;
            }
        }

        private int currentPage = 1;

        /// <summary>
        /// 最大显示的页码数
        /// </summary>
        [Parameter]
        public int ShowPageCount { get; set; } = 7;

        /// <summary>
        /// 当前最大显示的页码数变化时触发
        /// </summary>
        [Parameter]
        public EventCallback<int> ShowPageCountChanged { get; set; }

        /// <summary>
        /// 当页码变化时触发
        /// </summary>
        [Parameter]
        public EventCallback<int> CurrentPageChanged { get; set; }


        /// <summary>
        /// 当只有一页时，不显示分页
        /// </summary>
        [Parameter]
        public bool NoPaginationOnSinglePage { get; set; } = true;

        /// <summary>
        /// 选中的记录
        /// </summary>
        [Parameter]
        public HashSet<object> SelectedRows { get; set; } = new HashSet<object>();

        /// <summary>
        /// 选中的记录变化时触发
        /// </summary>
        [Parameter]
        public EventCallback<HashSet<object>> SelectedRowsChanged { get; set; }
        internal Status selectAllStatus;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// 启用分页
        /// </summary>
        [Parameter]
        public bool EnablePagination { get; set; } = true;

        /// <summary>
        /// 表格高度
        /// </summary>
        [Parameter]
        public int Height { get; set; }

        /// <summary>
        /// 启用斑马纹
        /// </summary>
        [Parameter]
        public bool IsStripe { get; set; }

        /// <summary>
        /// 加载中状态背景颜色
        /// </summary>
        [Parameter]
        public string LoadingBackground { get; set; }

        /// <summary>
        /// 加载中状态样式类
        /// </summary>
        [Parameter]
        public string LoadingIconClass { get; set; }

        /// <summary>
        /// 加载中状态文字
        /// </summary>
        public string LoadingText { get; set; }
        /// <summary>
        /// 当加载数据源时触发，传入参数为当前页
        /// </summary>
        [Parameter]
        public Func<int, Task<PagerResult>> OnLoadDataSource { get; set; }
        public ElementReference Container { get; set; }

        internal Task OnRowCheckBoxRenderCompleted(object row, BCheckBox<bool> chk)
        {
            lock (rowCheckBoxses)
            {
                rowCheckBoxses.Add(chk, row);
            }
            return Task.CompletedTask;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            SelectedRows = new HashSet<object>();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (renderCompleted)
            {
                if (dataSourceUpdated)
                {
                    dataSourceUpdated = false;
                    await RefreshDataSourceAsync();
                }
                return;
            }
            if (headerRendered)
            {
                renderCompleted = true;
                await (OnRenderCompleted?.Invoke(this) ?? Task.CompletedTask);
                return;
            }
            await RefreshDataSourceAsync();
            InitlizeDataSource(0, "up");
            headerRendered = true;
            RequireRender = true;
            StateHasChanged();
        }

        private async Task RefreshDataSourceAsync()
        {
            if (!string.IsNullOrWhiteSpace(Url))
            {
                DataSource = await httpClient.GetFromJsonAsync(Url, typeof(List<>).MakeGenericType(DataType));
                if (DataSourceChanged.HasDelegate)
                {
                    await DataSourceChanged.InvokeAsync(DataSource);
                }

                if (FormItem != null)
                {
                    var dyncmicDataSource = (List<KeyValueModel>)DataSource;
                    SetFieldValue(dyncmicDataSource.ToDictionary(x => x.Key, x => x.Value), true);
                }
            }
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (parameters.GetValueOrDefault<object>(nameof(DataSource)) != null)
            {
                InitlizeDataSource(0, string.Empty);
                return;
            }
        }

        private void InitlizeDataSource(int level, string direction)
        {
            if (DataSource != null)
            {
                rows = (DataSource as IEnumerable).Cast<object>().ToList();
                if (string.IsNullOrWhiteSpace(Url))
                {
                    if (rows.Any(x => x is ITreeItem))
                    {
                        var rootLevels = rows.Cast<ITreeItem>().Where(x => x.ParentId == 0).ToList();
                        CalculateLevel(rootLevels, -1, rows.Cast<ITreeItem>().ToArray());
                    }
                    DataType = DataSource.GetType().GetGenericArguments()[0];
                }
                else
                {
                    rows.Clear();
                    foreach (ITreeItem item in (DataSource as IEnumerable))
                    {
                        if (item.Level == null)
                        {
                            item.Level = level;
                            item.Direction = direction;
                        }
                        rows.Add(item);
                    }

                }
            }
            if (AutoGenerateColumns && !headerInitilized)
            {
                headerInitilized = true;
                DataType.GetProperties().Where(p => !IgnoreProperties.Contains(p.Name)).Reverse().ToList().ForEach(property =>
                {
                    if (Headers.Any(x => x.Property?.Name == property.Name))
                    {
                        return;
                    }
                    var attrs = property.GetCustomAttributes(true);
                    if (attrs.OfType<TableIgnoreAttribute>().Any())
                    {
                        Headers.Add(new TableHeader()
                        {
                            Ignore = true,
                            Property = property
                        });
                        return;
                    }
                    var columnConfig = attrs.OfType<TableColumnAttribute>().FirstOrDefault() ?? new TableColumnAttribute()
                    {
                        Text = property.Name
                    };

                    if (columnConfig.Ignore)
                    {
                        Headers.Add(new TableHeader()
                        {
                            Ignore = true,
                            Property = property
                        });
                        return;
                    }
                    var editorConfig = attrs.OfType<EditorAttribute>().FirstOrDefault() ?? new EditorAttribute()
                    {
                        Control = typeof(BInput<string>)
                    };

                    var formConfig = attrs.OfType<FormControlAttribute>().FirstOrDefault() ?? new FormControlAttribute()
                    {
                        IsRequired = true,
                        RequiredMessage = "该字段必填"
                    };
                    var propertyConfig = attrs.OfType<PropertyAttribute>().FirstOrDefault();
                    PropertyInfo entityProperty = null;
                    PropertyInfo dataTypeProperty = null;
                    if (propertyConfig != null)
                    {
                        dataTypeProperty = DataType.GetProperty(propertyConfig.ModelProperty);
                        entityProperty = dataTypeProperty.PropertyType.GetProperty(propertyConfig.Property);
                    }

                    var tableHeader = new TableHeader()
                    {
                        EvalRaw = row =>
                        {
                            object value = null;
                            if (entityProperty != null)
                            {
                                value = entityProperty.GetValue(dataTypeProperty.GetValue(row));
                            }
                            else
                            {
                                value = property.GetValue(row);
                            }
                            return value;
                        },
                        Eval = row =>
                        {
                            object value = null;
                            if (entityProperty != null)
                            {
                                value = entityProperty.GetValue(dataTypeProperty.GetValue(row));
                            }
                            else
                            {
                                value = property.GetValue(row);
                            }
                            if (value != null)
                            {
                                var valueType = value.GetType();
                                var finalType = Nullable.GetUnderlyingType(valueType) ?? valueType;
                                if (finalType.IsEnum)
                                {
                                    var enumObj = Convert.ChangeType(value, finalType).ToString();
                                    var attrs = finalType.GetField(enumObj).GetCustomAttributes();
                                    var display = (attrs.OfType<DescriptionAttribute>().FirstOrDefault()?.Description ?? attrs.OfType<DisplayAttribute>().FirstOrDefault()?.Description) ?? enumObj;
                                    value = display;
                                }
                            }
                            if (string.IsNullOrWhiteSpace(columnConfig.Format))
                            {
                                return value;
                            }
                            if (value == null)
                            {
                                return null;
                            }

                            try
                            {
                                return Convert.ToDateTime(value).ToString(columnConfig.Format);
                            }
                            catch (InvalidCastException)
                            {
                                throw new BlazuiException("仅日期列支持 Format 参数");
                            }
                        },
                        SortNo = columnConfig.SortNo,
                        IsCheckBox = property.PropertyType == typeof(bool) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(bool),
                        Property = property,
                        EntityProperty = entityProperty,
                        Text = columnConfig.Text,
                        Width = columnConfig.Width,
                        IsEditable = columnConfig.IsEditable
                    };
                    if (IsEditable && columnConfig.IsEditable)
                    {
                        var controlInfo = map.GetControl(property, entityProperty);
                        tableHeader.EditorRender = (IControlRender)provider.GetRequiredService(controlInfo.RenderType);
                        tableHeader.EditorRenderConfig = new RenderConfig()
                        {
                            InputControlType = controlInfo.ControlType,
                            IsRequired = editorConfig.IsRequired,
                            RequiredMessage = editorConfig.RequiredMessage ?? $"{tableHeader.Text}不能为空",
                            Placeholder = editorConfig.Placeholder,
                            Property = property,
                            EntityProperty = entityProperty,
                            DataSourceLoader = controlInfo.DataSourceLoader,
                            Page = Page
                        };
                    }
                    Headers.Insert(0, tableHeader);
                }
                 );
                var lastIndex = Headers.Count;
                var btnUpdateText = "更新";
                BButton btnUpdate = null, btnDelete = null;
                Headers.Insert(lastIndex++, new TableHeader()
                {
                    Text = "操作",
                    Template = row =>
                    {
                        return (builder) =>
                        {
                            builder.OpenComponent<BButtonGroup>(0);
                            builder.AddAttribute(1, nameof(BButtonGroup.ChildContent), (RenderFragment)(builder =>
                            {
                                builder.OpenComponent<BButton>(1);
                                builder.AddAttribute(2, nameof(BButton.Type), ButtonType.Primary);
                                builder.AddAttribute(3, nameof(BButton.Size), ButtonSize.Small);
                                builder.AddAttribute(4, nameof(BButton.ChildContent), (RenderFragment)(builder =>
                                {
                                    if (row == editingRow)
                                    {
                                        builder.AddMarkupContent(5, btnUpdateText);
                                    }
                                    else
                                    {
                                        builder.AddMarkupContent(5, "更新");
                                    }
                                }));
                                builder.AddAttribute(6, nameof(BButton.OnClick), EventCallback.Factory.Create<MouseEventArgs>(Page, async (e) =>
                                {
                                    if (editingTaskCompletionSource == null)
                                    {
                                        editingTaskCompletionSource = new TaskCompletionSource<int>();
                                        editingRow = row;
                                        btnUpdateText = "保存";
                                        btnUpdate.MarkAsRequireRender();
                                        Refresh();
                                        return;
                                    }
                                    btnUpdate.IsLoading = true;
                                    btnUpdate.Refresh();
                                    _ = SaveDataAsync(SaveAction.Update);
                                    var result = await editingTaskCompletionSource.Task;
                                    if (result != 0)
                                    {
                                        editingTaskCompletionSource = new TaskCompletionSource<int>();
                                        return;
                                    }
                                    editingTaskCompletionSource = null;
                                    dataSourceUpdated = true;
                                    Toast("更新成功");
                                    editingRow = null;
                                    btnUpdate.IsLoading = false;
                                    btnUpdateText = "更新";
                                    Refresh();
                                }));
                                builder.AddComponentReferenceCapture(7, btn => btnUpdate = (BButton)btn);
                                builder.CloseComponent();
                                builder.OpenComponent<BButton>(8);
                                builder.AddAttribute(9, nameof(BButton.Type), ButtonType.Danger);
                                builder.AddAttribute(10, nameof(BButton.Size), ButtonSize.Small);
                                builder.AddAttribute(11, nameof(BButton.ChildContent), (RenderFragment)(builder =>
                                {
                                    builder.AddMarkupContent(12, "删除");
                                }));
                                builder.AddAttribute(13, nameof(BButton.OnClick), EventCallback.Factory.Create<MouseEventArgs>(Page, async (e) =>
                                {
                                    var confirmResult = await ConfirmAsync("确认要删除吗？");
                                    if (confirmResult != MessageBoxResult.Ok)
                                    {
                                        return;
                                    }
                                    foreach (var item in Headers)
                                    {
                                        item.RawValue = row;
                                    }
                                    editingTaskCompletionSource = new TaskCompletionSource<int>();
                                    btnDelete.IsLoading = true;
                                    btnDelete.Refresh();
                                    _ = SaveDataAsync(SaveAction.Delete);
                                    var result = await editingTaskCompletionSource.Task;
                                    if (result != 0)
                                    {
                                        editingTaskCompletionSource = new TaskCompletionSource<int>();
                                        return;
                                    }
                                    editingTaskCompletionSource = null;
                                    dataSourceUpdated = true;
                                    Toast("删除成功");
                                    Refresh();
                                }));
                                builder.AddComponentReferenceCapture(14, btn => btnDelete = (BButton)btn);
                                builder.CloseComponent();
                            }));
                            builder.CloseComponent();
                        };
                    }
                }); ;
            }
            chkAll?.MarkAsRequireRender();
            ResetSelectAllStatus();
        }

        private void CalculateLevel(List<ITreeItem> treeItems, int parentLevel, ITreeItem[] allTreeItems)
        {
            foreach (var treeRow in treeItems)
            {
                if (treeRow.Level != null)
                {
                    return;
                }
                treeRow.Level = parentLevel + 1;
                var children = allTreeItems.Where(x => x.ParentId == treeRow.Id).ToList();
                CalculateLevel(children, treeRow.Level.Value, allTreeItems);
            }
        }

        private async Task SaveTableAsync(MouseEventArgs e)
        {
#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
            saveAddButton.IsLoading = true;
            saveAddButton.Refresh();
            await SaveDataAsync(SaveAction.Create);
            saveAddButton.IsLoading = false;
            saveAddButton.Refresh();
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.
        }

        private async Task SaveTableAsync(KeyboardEventArgs e)
        {
            if (e.Code != "Enter")
            {
                return;
            }
            var keyValueModels = DataSource as List<KeyValueModel>;
            if (string.IsNullOrWhiteSpace(Url) && (DataSource == null || keyValueModels != null))
            {
                var keyHeader = Headers.FirstOrDefault(x => x.Property.Name == "Key");
                var txtKey = keyHeader.EditingValue?.Trim();
                if (string.IsNullOrWhiteSpace(txtKey))
                {
                    return;
                }
                var valueHeader = Headers.FirstOrDefault(x => x.Property.Name == "Value");
                var txtValue = valueHeader.EditingValue?.Trim();
                txtKey = txtKey.Trim();
                if (keyValueModels == null)
                {
                    DataSource = keyValueModels = new List<KeyValueModel>();
                }
                var existKeyValueModel = keyValueModels.FirstOrDefault(x => x.Key == txtKey);
                if (existKeyValueModel == null)
                {
                    keyValueModels.Add(new KeyValueModel()
                    {
                        Key = txtKey,
                        Value = txtValue
                    });
                }
                else
                {
                    existKeyValueModel.Value = txtValue;
                }
                keyHeader.EditingValue = string.Empty;
                valueHeader.EditingValue = null;
                InitlizeDataSource(0, string.Empty);
                if (editingRow != null)
                {
                    editingRow = null;
                }
                if (DataSourceChanged.HasDelegate)
                {
                    await DataSourceChanged.InvokeAsync(DataSource);
                }
                if (FormItem != null)
                {
                    SetFieldValue(((List<KeyValueModel>)DataSource).ToDictionary(x => x.Key, x => x.Value), true);
                }
                return;
            }
            await SaveDataAsync(saveAction);
        }

        private async Task SaveDataAsync(SaveAction saveAction)
        {
            if (DataType == null)
            {
                if (DataSource == null)
                {
                    throw new BlazuiException("DataType 或 DataSource 必须设置一个");
                }
                DataType = DataSource.GetType().GetGenericArguments()[0];
            }
            var row = Activator.CreateInstance(DataType);
            foreach (var header in Headers)
            {
                if (!CanEdit(header))
                {
                    if (header.Property != null && header.Property.Name.Contains("Id"))
                    {
                        header.Property.SetValue(row, header.Property.GetValue(header.RawValue));
                    }
                    continue;
                }
                if (saveAction != SaveAction.Delete)
                {
                    object cell;
                    if (header.EntityProperty != null)
                    {
                        cell = Convert.ChangeType(header.EditorRenderConfig.EditingValue, header.EntityProperty.PropertyType);
                    }
                    else
                    {
                        cell = Convert.ChangeType(header.EditorRenderConfig.EditingValue, header.Property.PropertyType);
                    }
                    if (header.EditorRenderConfig.IsRequired && cell == null)
                    {
                        Toast(header.EditorRenderConfig.RequiredMessage);
                        editingTaskCompletionSource.TrySetResult(-1);
                        return;
                    }
                    header.Property.SetValue(row, cell);
                }
                header.RawValue = null;
                header.EditingValue = null;
                header.EditorRenderConfig.RawValue = null;
                header.EditorRenderConfig.RawLabel = null;
                header.EditorRenderConfig.RawInfoHasSet = false;
            }
            var tableSaveArg = new TableSaveEventArgs()
            {
                Action = saveAction,
                Data = row
            };
            if (editingTaskCompletionSource != null)
            {
                if (OnSave.HasDelegate)
                {
                    await OnSave.InvokeAsync(tableSaveArg);
                    if (tableSaveArg.Cancel)
                    {
                        editingTaskCompletionSource.TrySetResult(-1);
                        return;
                    }
                    editingTaskCompletionSource.TrySetResult(0);
                    return;
                }

                editingTaskCompletionSource.TrySetResult(-1);
            }
            else
            {
                if (OnSave.HasDelegate)
                {
                    await OnSave.InvokeAsync(new TableSaveEventArgs()
                    {
                        Action = SaveAction.Create,
                        Data = row
                    });
                }
            }
        }

        private bool CanEdit(TableHeader header)
        {
            return header.IsEditable && header.Eval != null;
        }

        private void BeginEdit(object row)
        {
            saveAction = SaveAction.Create;
            editingRow = row;
            MarkAsRequireRender();
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            DataSource = ((IDictionary<string, string>)value).Select(x => new KeyValueModel()
            {
                Key = x.Key,
                Value = x.Value
            }).ToList();
            if (requireRerender)
            {
                MarkAsRequireRender();
            }
        }

        private async Task ToggleTreeAsync(object row)
        {
            var treeRow = row as ITreeItem;
            if (treeRow == null || treeRow.IsLoading)
            {
                return;
            }
            var treeRows = rows.Cast<ITreeItem>().ToArray();
            var children = treeRows.Where(x => x.ParentId == treeRow.Id).ToList();
            var finalChildren = children.ToList();
            foreach (var item in children)
            {
                finalChildren.AddRange(FindChildren(item));
            }
            if (string.IsNullOrWhiteSpace(Url))
            {
                if (hiddenRows.Any(x => finalChildren.Contains(x)))
                {
                    hiddenRows.RemoveAll(finalChildren.Contains);
                    treeRow.Direction = "right";
                }
                else
                {
                    hiddenRows.AddRange(finalChildren);
                    treeRow.Direction = "up";
                }
            }
            else
            {
                if (finalChildren.Any())
                {
                    rows.RemoveAll(finalChildren.Contains);
                    DataSource = rows;
                    InitlizeDataSource(0, "up");
                    if (DataSourceChanged.HasDelegate)
                    {
                        await DataSourceChanged.InvokeAsync(DataSource);
                    }
                    if (FormItem != null)
                    {
                        SetFieldValue(((List<KeyValueModel>)DataSource).ToDictionary(x => x.Key, x => x.Value), true);
                    }
                    treeRow.Direction = "up";
                }
                else
                {
                    treeRow.IsLoading = true;
                    MarkAsRequireRender();
                    Refresh();
                    object newDataSource = await FetchChildrenAsync(treeRow);
                    treeRow.IsLoading = false;
                    var enumerbale = DataSource as IEnumerable;
                    var startIndex = 0;
                    foreach (var item in enumerbale)
                    {
                        if (item == row)
                        {
                            break;
                        }
                        startIndex++;
                    }
                    var oldDataSource = enumerbale.Cast<object>().ToList();
                    oldDataSource.InsertRange(startIndex + 1, (newDataSource as IEnumerable).Cast<object>());
                    DataSource = oldDataSource;
                    InitlizeDataSource(treeRow.Level.Value + 1, "up");
                    if (DataSourceChanged.HasDelegate)
                    {
                        await DataSourceChanged.InvokeAsync(DataSource);
                    }
                    if (FormItem != null)
                    {
                        SetFieldValue(((List<KeyValueModel>)DataSource).ToDictionary(x => x.Key, x => x.Value), true);
                    }
                }
            }
            MarkAsRequireRender();
        }

        private async Task<object> FetchChildrenAsync(ITreeItem treeRow)
        {
            treeRow.Direction = "right";
            if (!Url.EndsWith("/"))
            {
                Url += "/";
            }

            object newDataSource = await httpClient.GetFromJsonAsync(Url + treeRow.Id, typeof(List<>).MakeGenericType(DataType));
            return newDataSource;
        }

        public override void MarkAsRequireRender()
        {
            base.MarkAsRequireRender();
            if (editingRow == null && !renderCompleted)
            {
                headerRendered = false;
                headerInitilized = false;
            }
        }

        private List<ITreeItem> FindChildren(ITreeItem child)
        {
            var results = new List<ITreeItem>();
            var treeItems = rows.Cast<ITreeItem>().Where(x => x.ParentId == child.Id).ToArray();
            foreach (var childTreeItem in treeItems)
            {
                results.Add(childTreeItem);
                results.AddRange(FindChildren(childTreeItem));
            }
            return results;
        }

        internal async Task CurrentPageChangedAsync(int page)
        {
            CurrentPage = page;
            if (CurrentPageChanged.HasDelegate)
            {
                RequireRender = true;
                var option = new LoadingOption()
                {
                    Background = LoadingBackground,
                    Target = Container,
                    IconClass = LoadingIconClass,
                    Text = LoadingText
                };
                LoadingService.LoadingOptions.Add(option);
                SelectedRows.Clear();
                await CurrentPageChanged.InvokeAsync(page);
                LoadingService.LoadingOptions.Remove(option);
            }
        }

        void ResetSelectAllStatus()
        {
            if (rows.Count == 0 || SelectedRows.Count == 0)
            {
                selectAllStatus = Status.UnChecked;
            }
            else if (rows.Count > SelectedRows.Count)
            {
                selectAllStatus = Status.Indeterminate;
            }
            else
            {
                selectAllStatus = Status.Checked;
            }
        }

        protected void ChangeAllStatus(Status status)
        {
            RequireRender = true;
            if (status == Status.Checked)
            {
                SelectedRows = new HashSet<object>(rows);
            }
            else
            {
                SelectedRows = new HashSet<object>();
            }

            foreach (var item in rowCheckBoxses.Keys)
            {
                item.MarkAsRequireRender();
            }
            if (SelectedRowsChanged.HasDelegate)
            {
                _ = SelectedRowsChanged.InvokeAsync(SelectedRows);
            }
            ResetSelectAllStatus();
        }

        protected void ChangeRowStatus(Status status, object row)
        {
            RequireRender = true;
            chkAll.MarkAsRequireRender();
            if (status == Status.Checked)
            {
                SelectedRows.Add(row);
            }
            else
            {
                SelectedRows.Remove(row);
            }
            if (SelectedRowsChanged.HasDelegate)
            {
                _ = SelectedRowsChanged.InvokeAsync(SelectedRows);
            }
            ResetSelectAllStatus();
        }
    }
}
