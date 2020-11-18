using Element;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Element.Admin
{
    public class BAdminBase : BAdminPageBase
    {
        internal BTab tab;
        internal BBreadcrumb breadcrumb;
        [Inject]
        private RouteService routeService { get; set; }

        protected BForm form;

        [Inject]
        private Element.MessageBox MessageBox { get; set; }

        protected string defaultMenuIndex;

        [Parameter]
        public LoginInfoModel DefaultUser { get; set; }

        [Parameter]
        public bool EnablePermissionMenus { get; set; } = false;
        protected string username;
        [Parameter]
        public Type LoginPage { get; set; }
        [Parameter]
        public Type CreatePage { get; set; }
        [Parameter]
        public Type ModifyPasswordPage { get; set; }

        [Parameter]
        public float NavigationWidth { get; set; } = 250;

        [Parameter]
        public bool EnableCodeGen { get; set; }

        /// <summary>
        /// 导航菜单栏标题
        /// </summary>
        [Parameter]
        public string NavigationTitle { get; set; } = "Element.Admin 后台模板";

        /// <summary>
        /// 面包屑标题
        /// </summary>
        [Parameter]
        public string BreadcrumbTitle { get; set; } = "首页";
        [Parameter]
        public List<MenuModel> Menus { get; set; }

        /// <summary>
        /// 右上角下拉框菜单
        /// </summary>
        [Parameter]
        public RenderFragment PersonMenus { get; set; }

        [Parameter]
        public string DefaultRoute { get; set; }

        internal string ActiveTabName { get; set; }

        [Parameter]
        public RenderFragment Body { get; set; }

        protected IMenuItem CurrentMenu { get; set; }

        /// <summary>
        /// 页面刚刚加载完成时自动加载选项卡的动作是否完成
        /// </summary>
        private bool isLoadRendered = false;

        internal async Task ModifyPasswordAsync()
        {
            var result = await DialogService.ShowDialogAsync<ModifyPasswordModel>((ModifyPasswordPage ?? typeof(BModifyPassword)), "修改密码", 500);
            if (result.Result == null)
            {
                return;
            }

            await UserService.LogoutAsync(form, NavigationManager.Uri);
        }

        internal async System.Threading.Tasks.Task LogoutAsync()
        {
            var result = await MessageBox.ConfirmAsync("是否确认注销登录？");
            if (result != MessageBoxResult.Ok)
            {
                return;
            }

            await UserService.LogoutAsync(form, NavigationManager.Uri);
        }
        /// <summary>
        /// 初始 Tab 集合
        /// </summary>
        [Parameter]
        public ObservableCollection<TabOption> Tabs { get; set; } = new ObservableCollection<TabOption>();

        protected void OnTabPanelChanging(BChangeEventArgs<BTabPanel> args)
        {
            args.DisallowChange = true;
            NavigationManager.NavigateTo(args.NewValue.Name);
        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var path = new Uri(NavigationManager.Uri).LocalPath;
            if (path == "/" && !string.IsNullOrWhiteSpace(DefaultRoute))
            {
                NavigationManager.NavigateTo(DefaultRoute);
                return;
            }

            FixMenuInfo(Menus);
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            username = user.Identity.Name;
            NavigationManager.LocationChanged -= NavigationManager_LocationChanged;
            NavigationManager.LocationChanged += NavigationManager_LocationChanged;

            if (EnablePermissionMenus)
            {
                var permissionMenu = new MenuModel();
                permissionMenu.Label = "权限管理";
                permissionMenu.Name = "权限管理";
                permissionMenu.Icon = "el-icon-lock";
                permissionMenu.Children.Add(new MenuModel()
                {
                    Icon = "el-icon-user-solid",
                    Label = "用户列表",
                    Route = "/user/list",
                    Name = "userlist",
                    Title = "用户列表"
                });
                permissionMenu.Children.Add(new MenuModel()
                {
                    Icon = "el-icon-s-custom",
                    Label = "角色列表",
                    Route = "/user/roles",
                    Name = "rolelist",
                    Title = "角色列表"
                });
                Menus.Add(permissionMenu);
            }

            if (EnableCodeGen)
            {
                var codeGenMenu = new MenuModel()
                {
                    Label = "代码生成器",
                    Name = "代码生成器"
                };
                codeGenMenu.Children.Add(new MenuModel()
                {
                    Label = "一键整站"
                });
                codeGenMenu.Children.Add(new MenuModel()
                {
                    Label = "一键单表",
                    Title = "一键生成单表CRUD",
                    Route = "/gen/table"
                });
                Menus.Add(codeGenMenu);
            }
            FindMenuName(Menus, path);
        }

        /// <summary>
        /// 菜单是否需要隐藏
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        internal bool RequireHide(MenuModel menu)
        {
            if (menu.Route == null)
            {
                return false;
            }
            var type = routeService.GetComponent(menu.Route);
            if (type == null)
            {
                return false;
            }
            var resource = type.GetCustomAttributes(false).OfType<ResourceAttribute>().FirstOrDefault();
            if (resource == null)
            {
                return false;
            }
            return !IsCanAccessAny(resource.Id);
        }


        /// <summary>
        /// 父菜单是否需要隐藏
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        internal bool RequireHide(List<MenuModel> children)
        {
            foreach (var item in children)
            {
                if (!RequireHide(item))
                {
                    return false;
                }
            }
            return true;
        }

        private void NavigationManager_LocationChanged(object sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            var path = new Uri(e.Location).LocalPath;
            AddTab(path);
        }

        void FindMenuName(List<MenuModel> menus, string path)
        {
            foreach (var menu in menus)
            {
                if (menu.Route == path)
                {
                    defaultMenuIndex = menu.Name;
                    return;
                }
                FindMenuName(menu.Children, path);
            }
        }

        void FixMenuInfo(List<MenuModel> menus)
        {
            foreach (var menu in menus)
            {
                menu.Name = menu.Name ?? menu.Route;
                menu.Title = menu.Title ?? menu.Label;
                FixMenuInfo(menu.Children);
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (!isLoadRendered)
            {
                isLoadRendered = true;
                var path = new Uri(NavigationManager.Uri).LocalPath;
                AddTab(path);
            }
        }

        private void AddTab(string path)
        {
            var type = routeService.GetComponent(path);
            if (type == null)
            {
                return;
            }
            ActiveTabName = path;
            foreach (var item in Tabs)
            {
                item.IsActive = false;
            }
            var activeTab = Tabs.FirstOrDefault(x => x.Name == ActiveTabName);
            if (activeTab == null)
            {
                if (CurrentMenu == null)
                {
                    return;
                }
                var model = (MenuModel)CurrentMenu.Model;
                Tabs.Add(new TabOption()
                {
                    Title = model.Title ?? model.Label,
                    IsClosable = true,
                    IsActive = true,
                    BodyStyle = model.Flex ? "display:flex;" : "display:block;",
                    Name = ActiveTabName,
                    Content = type
                });
            }
            else
            {
                activeTab.IsActive = true;
            }
            tab?.MarkAsRequireRender();
            tab?.Refresh();
            breadcrumb?.Refresh();

        }
    }
}
