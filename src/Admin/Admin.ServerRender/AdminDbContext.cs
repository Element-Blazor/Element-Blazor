using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Admin.ServerRender
{
    public class AdminDbContext : IdentityDbContext
    {
        private DbContextOptions options;

        public AdminDbContext(DbContextOptions options)
        {
            this.options = options;
        }

        public DbSet<RoleResource> RoleResources { get; set; }
    }

    public class AdminDbContext<TUser> : IdentityDbContext<TUser>
        where TUser : IdentityUser
    {
        private DbContextOptions options;

        public AdminDbContext(DbContextOptions options)
        {
            this.options = options;
        }
        public DbSet<RoleResource> RoleResources { get; set; }
    }

    public class AdminDbContext<TUser, TRole, TKey> : IdentityDbContext<TUser, TRole, TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        private DbContextOptions options;

        public AdminDbContext(DbContextOptions options)
        {
            this.options = options;
        }
        public DbSet<RoleResource> RoleResources { get; set; }
    }
}
