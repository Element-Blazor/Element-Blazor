using Blazui.Component.ControlConfigs;
using Blazui.Component.ControlRender;
using Blazui.Component.ControlRenders;
using Blazui.Component.DisplayRenders;
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
using System.Threading;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public partial class BTable : IContainerComponent
    {
        private SaveAction saveAction = SaveAction.Create;
        private TaskCompletionSource<int> editingTaskCompletionSource;
        private TaskCompletionSource<int> loadTaskCompletionSource;
        private bool dataSourceUpdated = false;
        private BButton saveAddButton;
        private bool renderCompleted = false;
        private ElementReference editingRowEl;
        internal BCheckBox<bool> chkAll;
        internal ElementReference headerElement;
        private object editingRow;

        /// <summary>
        /// 主键字段
        /// </summary>
        [Parameter]
        public string Key { get; set; }

        [Inject]
        private DisplayRenderFactory displayRender { get; set; }
        internal List<TableHeader> Headers { get; set; } = new List<TableHeader>();

        internal bool headerInitilized = false;
        internal bool headerRendered = false;
        private IDictionary<BCheckBox<bool>, object> rowCheckBoxses = new Dictionary<BCheckBox<bool>, object>();
        internal int headerHeight = 49;
        internal List<object> rows = new List<object>();
        internal List<object> hiddenRows = new List<object>();

        /// <summary>
        /// 自动展开所有节点
        /// </summary>
        [Parameter]
        public bool AutoExpandAll { get; set; } = true;

        /// <summary>
        /// 节点并行加载
        /// </summary>
        [Parameter]
        public bool EnableParallel { get; set; } = true;

        [Parameter]
        public EventCallback<object> OnRowDblClick { get; set; }

        [Inject]
        private TableEditorMap map { get; set; }

        [Inject]
        private IServiceProvider provider { get; set; }

        [Inject]
        private HttpClient httpClient { get; set; }

        public async Task RefreshNodeAsync(TreeItemBase item)
        {
            await ExpandAsync(item, true);
        }

        /// <summary>
        /// 刷新根节点
        /// </summary>
        public async Task RefreshRootNodeAsync()
        {
            if (!rows.Any())
            {
                return;
            }
            await RefreshNodeAsync((TreeItemBase)rows[0]);
        }

        /// <summary>
        /// 展开某个节点，如果这个节点当前没有子节点，则动态加载子节点
        /// </summary>
        /// <param name="item"></param>
        /// <param name="forceLoad">是否强制加载新数据</param>
        public async Task ExpandAsync(TreeItemBase item, bool forceLoad = false)
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                ExceptionHelper.Throw(ExceptionHelper.ExpandOnlyHasUrl, "展开某个节点只支持异步加载");
            }
            ResetSelectAllStatus();
            item.Expanded = true;
            var finalChildren = GetAllChildren(item);
            if (forceLoad || !finalChildren.Any())
            {
                if (!forceLoad)
                {
                    item.HasChildren = true;
                }
                if (finalChildren.Count != rows.RemoveAll(finalChildren.Contains))
                {
                    ExceptionHelper.Throw(ExceptionHelper.ClearChildFailure, "清空子节点失败");
                }
                DataSource = rows;
                await LoadChildrenAsync(item, AutoExpandAll);
                Refresh();
                return;
            }

            LocalExpand(item, finalChildren);
            Refresh();
        }

        private async Task ForceExpandAsync(TreeItemBase item)
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                ExceptionHelper.Throw(ExceptionHelper.ExpandOnlyHasUrl, "展开某个节点只支持异步加载");
            }
            item.Expanded = true;
            var finalChildren = GetAllChildren(item);
            item.HasChildren = true;
            rows.RemoveAll(finalChildren.Contains);
            await LoadChildrenAsync(item, AutoExpandAll);
            Refresh();
        }

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
        /// 若实体实现了 <seealso cref="INotifyPropertyChanged"/> 接口，此参数控制多少毫秒没有新通知后刷新数据
        /// </summary>
        [Parameter]
        public int BatchUpdateDelayTime { get; set; } = 1000;

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
        private CancellationTokenSource delayCancelToken;
        private Task delayTask;

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
                    await RefreshDataSourceAsync(AutoExpandAll);
                }
                return;
            }
            if (headerRendered)
            {
                renderCompleted = true;
                await (OnRenderCompleted?.Invoke(this) ?? Task.CompletedTask);
                return;
            }
            await RefreshDataSourceAsync(AutoExpandAll);
            headerRendered = true;
            RequireRender = true;
            StateHasChanged();
        }

        internal async Task RefreshDataSourceAsync(bool autoExpand)
        {
            if (!string.IsNullOrWhiteSpace(Url))
            {
                DataSource = await httpClient.GetFromJsonAsync(Url, typeof(List<>).MakeGenericType(DataType));
                if (DataSourceChanged.HasDelegate)
                {
                    await DataSourceChanged.InvokeAsync(DataSource);
                }

                SyncFieldValue();
            }
            await InitlizeDataSourceAsync(0, "up", autoExpand);
        }

        private void SyncFieldValue()
        {
            if (FormItem == null)
            {
                return;
            }
            var dyncmicDataSource = (List<KeyValueModel>)DataSource;
            var fieldValue = dyncmicDataSource.ToDictionary(x => x.Key, x => x.Value);
            SetFieldValue(fieldValue, true);
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (parameters.GetValueOrDefault<object>(nameof(DataSource)) != null)
            {
                await InitlizeDataSourceAsync(0, string.Empty, true);
                return;
            }
        }

        private async Task InitlizeDataSourceAsync(int level, string direction, bool autoExpand)
        {
            if (DataSource != null)
            {
                rows = (DataSource as IEnumerable).Cast<object>().ToList();
                foreach (var item in rows)
                {
                    var propertyChanged = item as INotifyPropertyChanged;
                    if (propertyChanged == null)
                    {
                        break;
                    }
                    propertyChanged.PropertyChanged -= PropertyChanged_PropertyChanged;
                    propertyChanged.PropertyChanged += PropertyChanged_PropertyChanged;
                }
                if (string.IsNullOrWhiteSpace(Url))
                {
                    if (rows.Any(x => x is TreeItemBase))
                    {
                        var rootLevels = rows.Cast<TreeItemBase>().Where(x => x.ParentId == 0).ToList();
                        CalculateLevel(rootLevels, null, rows.Cast<TreeItemBase>().ToArray());
                    }
                    DataType = DataSource.GetType().GetGenericArguments()[0];
                }
                else
                {
                    rows.Clear();
                    foreach (TreeItemBase item in (DataSource as IEnumerable))
                    {
                        if (item.Level == null)
                        {
                            item.Level = level;
                            item.Direction = direction;
                        }
                        rows.Add(item);
                        if (AutoExpandAll && autoExpand && item.HasChildren && !item.Expanded)
                        {
                            var task = ForceExpandAsync(item);
                            if (!EnableParallel)
                            {
                                await task;
                            }
                            _ = ReadyExecuteRefreshAsync();
                        }
                    }

                }
            }
            else
            {
                DataSource = Activator.CreateInstance(typeof(List<>).MakeGenericType(DataType));
            }
            InitilizeHeaders();
        }

        private void InitilizeHeaders()
        {
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
                    var editorConfig = attrs.OfType<EditorGeneratorAttribute>().FirstOrDefault() ?? new EditorGeneratorAttribute()
                    {
                        Control = typeof(BInput<string>)
                    };

                    var formConfig = attrs.OfType<FormControlAttribute>().FirstOrDefault();
                    var propertyConfig = attrs.OfType<PropertyAttribute>().FirstOrDefault();

                    var tableHeader = new TableHeader()
                    {
                        EvalRaw = row =>
                        {
                            object value = property.GetValue(row);
                            return value;
                        },
                        SortNo = columnConfig.SortNo,
                        IsCheckBox = property.PropertyType == typeof(bool) || Nullable.GetUnderlyingType(property.PropertyType) == typeof(bool),
                        Property = property,
                        Text = columnConfig.Text,
                        Width = columnConfig.Width,
                        IsEditable = columnConfig.IsEditable
                    };
                    tableHeader.Eval = displayRender.CreateRenderFactory(tableHeader).CreateRender(tableHeader);
                    if (IsEditable && columnConfig.IsEditable)
                    {
                        InitilizeHeaderEditor(property, editorConfig, tableHeader);
                    }
                    Headers.Insert(0, tableHeader);
                }
                 );
                if (IsEditable)
                {
                    CreateOperationColumn();
                }
                chkAll?.MarkAsRequireRender();
                ResetSelectAllStatus();
            }
            else if (!AutoGenerateColumns && !headerInitilized && Headers.Any())
            {
                headerInitilized = true;
                foreach (var header in Headers)
                {
                    if (!CanEdit(header))
                    {
                        continue;
                    }
                    InitilizeHeaderEditor(header.Property, header.Property.GetCustomAttribute<EditorGeneratorAttribute>() ?? new EditorGeneratorAttribute(), header);
                }
                CreateOperationColumn();
                Refresh();
            }
        }

        private void CreateOperationColumn()
        {
            Headers.RemoveAll(x => x.IsOperation);
            var lastIndex = Headers.Count;
            var operationHeader = new TableHeader()
            {
                Text = "操作",
                IsOperation = true
            };
            BButton btnUpdate = null, btnDelete = null;
            operationHeader.Template = row =>
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
                                builder.AddMarkupContent(5, "保存");
                            }
                            else
                            {
                                builder.AddMarkupContent(5, "更新");
                            }
                        }));
                        if (Page == null)
                        {
                            throw new BlazuiException(1, "表格启用可编辑功能后必须在外面套一层 CascadingValue，值为 BDialogBase(this)，名称为 Page");
                        }
                        builder.AddAttribute(6, nameof(BButton.OnClick), EventCallback.Factory.Create<MouseEventArgs>(Page, async (e) =>
                        {
                            if (editingTaskCompletionSource == null)
                            {
                                editingTaskCompletionSource = new TaskCompletionSource<int>();
                                editingRow = row;
                                btnUpdate.MarkAsRequireRender();
                                Refresh();
                                return;
                            }
                            btnUpdate.IsLoading = true;
                            Refresh();
                            _ = SaveDataAsync(SaveAction.Update);
                            var result = await editingTaskCompletionSource.Task;
                            editingTaskCompletionSource = null;
                            btnUpdate.IsLoading = false;
                            if (result != 0)
                            {
                                Refresh();
                                editingTaskCompletionSource = new TaskCompletionSource<int>();
                                return;
                            }
                            foreach (var item in Headers)
                            {
                                item.EditingValue = (string)(item.EditorRenderConfig == null ? null : item.EditorRenderConfig.EditingValue = null);
                            }
                            btnUpdate.MarkAsRequireRender();
                            Refresh();
                        }));
                        builder.AddComponentReferenceCapture(7, btn =>
                        {
                            if (row == editingRow || editingRow == null)
                            {
                                btnUpdate = (BButton)btn;
                            }
                        });
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
                            btnDelete.IsLoading = false;
                            editingTaskCompletionSource = null;
                            if (result != 0)
                            {
                                btnDelete.Refresh();
                                editingTaskCompletionSource = new TaskCompletionSource<int>();
                                return;
                            }
                            editingTaskCompletionSource = null;
                            editingRow = null;
                            Toast("删除成功");
                        }));
                        builder.AddComponentReferenceCapture(14, btn => btnDelete = (BButton)btn);
                        builder.CloseComponent();
                    }));
                    builder.CloseComponent();
                };
            };
            Headers.Insert(lastIndex++, operationHeader);
        }

        private void InitilizeHeaderEditor(PropertyInfo property, EditorGeneratorAttribute editorConfig, TableHeader tableHeader)
        {
            var controlInfo = map.GetControl(property);
            tableHeader.EditorRender = (IControlRender)provider.GetRequiredService(controlInfo.RenderType);
            tableHeader.EditorRenderConfig = new RenderConfig()
            {
                InputControlType = controlInfo.ControlType,
                IsRequired = editorConfig.IsRequired,
                RequiredMessage = editorConfig.RequiredMessage ?? $"{tableHeader.Text}不能为空",
                Placeholder = editorConfig.Placeholder,
                Property = property,
                DataSourceLoader = controlInfo.DataSourceLoader,
                Page = Page
            };
        }

        private async Task ReadyExecuteRefreshAsync()
        {
            if (loadTaskCompletionSource == null)
            {
                loadTaskCompletionSource = new TaskCompletionSource<int>();
            }
            else
            {
                loadTaskCompletionSource.TrySetResult(0);
                loadTaskCompletionSource.TrySetCanceled();
                loadTaskCompletionSource = null;
                await ReadyExecuteRefreshAsync();
                return;
            }
            var task = Task.Delay(500);
            var resultTask = await Task.WhenAny(task, loadTaskCompletionSource.Task);
            if (resultTask == loadTaskCompletionSource.Task)
            {
                return;
            }
            await InvokeAsync(Refresh);
        }

        private void PropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (delayTask == null)
            {
                delayCancelToken = new CancellationTokenSource();
                delayTask = Task.Delay(BatchUpdateDelayTime, delayCancelToken.Token).ContinueWith(t =>
                {
                    if (t.IsCanceled)
                    {
                        return;
                    }
                    dataSourceUpdated = true;
                    InvokeAsync(Refresh);
                    delayTask = null;
                });
            }
            else if (delayTask.Status == TaskStatus.Running)
            {
                delayCancelToken.Dispose();
            }

        }

        private void CalculateLevel(List<TreeItemBase> treeItems, TreeItemBase parentNode, TreeItemBase[] allTreeItems)
        {
            foreach (var treeRow in treeItems)
            {
                if (treeRow.Level != null)
                {
                    return;
                }
                treeRow.Level = (parentNode?.Level ?? -1) + 1;
                treeRow.TextPaths = parentNode?.TextPaths == null ? new Dictionary<int, string>() : new Dictionary<int, string>(parentNode.TextPaths);
                if (!treeRow.TextPaths.ContainsKey(treeRow.Id))
                {
                    treeRow.TextPaths.Add(treeRow.Id, treeRow.Text);
                }
                var children = allTreeItems.Where(x => x.ParentId == treeRow.Id).ToList();
                CalculateLevel(children, treeRow, allTreeItems);
            }
        }

        private async Task SaveTableAsync(MouseEventArgs e)
        {
#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
            saveAddButton.IsLoading = true;
            saveAddButton.Refresh();
            await SaveDataAsync(SaveAction.Create);
            saveAddButton.IsLoading = false;
            saveAddButton.MarkAsRequireRender();
            MarkAsRequireRender();
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
                await InitlizeDataSourceAsync(0, string.Empty, true);
                if (editingRow != null)
                {
                    editingRow = null;
                }
                if (DataSourceChanged.HasDelegate)
                {
                    await DataSourceChanged.InvokeAsync(DataSource);
                }
                SyncFieldValue();
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
                    throw new BlazuiException(2, "DataType 或 DataSource 必须设置一个");
                }
                DataType = DataSource.GetType().GetGenericArguments()[0];
            }
            var row = Activator.CreateInstance(DataType);
            foreach (var header in Headers)
            {
                if ((!header.IsEditable && header.EditorRenderConfig == null) || (saveAction != SaveAction.Create && !CanEdit(header)))
                {
                    if (header.Property != null && (header.Property.Name.Contains("Id") || header.Property.Name.Contains("Key")) && saveAction != SaveAction.Create)
                    {
                        header.Property.SetValue(row, header.Property.GetValue(header.RawValue));
                    }
                    Clear(header);
                    continue;
                }
                if (saveAction != SaveAction.Delete)
                {
                    object cell = Convert.ChangeType(header.EditorRenderConfig.EditingValue, header.Property.PropertyType);
                    if (header.EditorRenderConfig.IsRequired && cell == null)
                    {
                        Toast(header.EditorRenderConfig.RequiredMessage);
                        editingTaskCompletionSource?.TrySetResult(-1);
                        return;
                    }
                    header.Property.SetValue(row, cell);
                }
                Clear(header);
            }
            var tableSaveArg = new TableSaveEventArgs()
            {
                Action = saveAction,
                Data = row,
                Table = this,
            };
            if (!OnSave.HasDelegate)
            {
                if (DataType == typeof(KeyValueModel))
                {
                    OnSave = EventCallback.Factory.Create<TableSaveEventArgs>(this, TableRender.DefaultSaverAsync);
                }
                else if (!string.IsNullOrWhiteSpace(Key))
                {
                    OnSave = EventCallback.Factory.Create<TableSaveEventArgs>(this, TableRender.DefaultSaverAsync);
                }
                else
                {
                    Toast("OnSave 事件没有注册或数据源类型不是 KeyValueModel 或没指定 Key");
                    editingTaskCompletionSource?.TrySetResult(-1);
                    return;
                }
            }
            tableSaveArg.Key = Key;
            tableSaveArg.DataType = DataType;
            if (editingTaskCompletionSource != null)
            {
                await OnSave.InvokeAsync(tableSaveArg);
                if (tableSaveArg.Cancel)
                {
                    editingTaskCompletionSource.TrySetResult(-1);
                    return;
                }
                editingRow = null;
                dataSourceUpdated = true;
                await RefreshDataSourceAsync(false);
                editingTaskCompletionSource.TrySetResult(0);
                SyncFieldValue();
            }
            else
            {
                if (OnSave.HasDelegate)
                {
                    await OnSave.InvokeAsync(tableSaveArg);
                    if (tableSaveArg.Cancel)
                    {
                        editingTaskCompletionSource?.TrySetResult(-1);
                        return;
                    }
                    editingRow = null;
                    dataSourceUpdated = true;
                    await RefreshDataSourceAsync(false);
                    editingTaskCompletionSource?.TrySetResult(0);
                    SyncFieldValue();
                    return;
                }
                editingTaskCompletionSource?.TrySetResult(-1);
            }
        }

        private void Clear(TableHeader header)
        {
            header.RawValue = null;
            header.EditingValue = null;
            if (header.EditorRenderConfig == null)
            {
                return;
            }
            header.EditorRenderConfig.RawValue = null;
            header.EditorRenderConfig.EditingValue = null;
            header.EditorRenderConfig.RawLabel = null;
            header.EditorRenderConfig.RawInfoHasSet = false;
        }

        private bool CanEdit(TableHeader header)
        {
            return header.IsEditable && header.Eval != null;
        }

        private async Task BeginEditAsync(object row)
        {
            if (OnRowDblClick.HasDelegate)
            {
                await OnRowDblClick.InvokeAsync(row);
            }
            if (!IsEditable)
            {
                return;
            }
            saveAction = SaveAction.Create;
            editingRow = row;
            MarkAsRequireRender();
        }

        private async Task ToggleTreeAsync(object row)
        {
            var treeRow = row as TreeItemBase;
            if (treeRow == null || treeRow.IsLoading)
            {
                return;
            }
            var finalChildren = GetAllChildren(treeRow);
            if (string.IsNullOrWhiteSpace(Url))
            {
                if (!treeRow.Expanded)
                {
                    LocalExpand(treeRow, finalChildren);
                }
                else
                {
                    hiddenRows.AddRange(finalChildren);
                    treeRow.Direction = "up";
                    treeRow.Expanded = false;
                }
            }
            else
            {
                if (treeRow.Expanded)
                {
                    treeRow.Expanded = false;
                    rows.RemoveAll(finalChildren.Contains);
                    DataSource = rows;
                    await InitlizeDataSourceAsync(0, "up", false);
                    if (DataSourceChanged.HasDelegate)
                    {
                        await DataSourceChanged.InvokeAsync(DataSource);
                    }
                    SyncFieldValue();
                    treeRow.Direction = "up";
                }
                else
                {
                    await LoadChildrenAsync(treeRow, false);
                }
            }
            MarkAsRequireRender();
        }

        private void LocalExpand(TreeItemBase treeRow, List<TreeItemBase> finalChildren)
        {
            hiddenRows.RemoveAll(finalChildren.Contains);
            treeRow.Direction = "right";
            treeRow.Expanded = true;
        }

        private List<TreeItemBase> GetAllChildren(TreeItemBase treeRow)
        {
            var treeRows = rows.Cast<TreeItemBase>().ToArray();
            var children = treeRows.Where(x => x.ParentId == treeRow.Id).ToList();
            var finalChildren = children.ToList();
            foreach (var item in children)
            {
                finalChildren.AddRange(FindChildren(item));
            }

            return finalChildren;
        }

        private async Task LoadChildrenAsync(TreeItemBase row, bool autoExpand)
        {
            row.Expanded = true;
            row.IsLoading = true;
            Refresh();
            var newDataSource = await FetchChildrenAsync(row);
            row.IsLoading = false;
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
            await InitlizeDataSourceAsync(row.Level.Value + 1, "up", autoExpand);
            if (DataSourceChanged.HasDelegate)
            {
                await DataSourceChanged.InvokeAsync(DataSource);
            }
            SyncFieldValue();
        }

        private async Task<IEnumerable> FetchChildrenAsync(TreeItemBase treeRow)
        {
            treeRow.Direction = "right";
            if (!Url.EndsWith("/"))
            {
                Url += "/";
            }

            var newDataSource = (IEnumerable)await httpClient.GetFromJsonAsync(Url + treeRow.Id, typeof(List<>).MakeGenericType(DataType));
            if (!newDataSource.Cast<object>().Any())
            {
                treeRow.HasChildren = false;
            }
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

        private List<TreeItemBase> FindChildren(TreeItemBase child)
        {
            var results = new List<TreeItemBase>();
            var treeItems = rows.Cast<TreeItemBase>().Where(x => x.ParentId == child.Id).ToArray();
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
