using Blazui.Admin;
using Blazui.Admin.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Admin.Sample.ClientRender.PWA.Shared.IServices
{
    public interface IUserServiceExtension : IUserService
    {
        Task<List<RoleResource>> GetResourcesAsync();
    }
}
