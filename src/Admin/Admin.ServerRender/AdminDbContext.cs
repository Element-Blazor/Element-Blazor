using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element.Admin.ServerRender
{
    public class AdminDbContext : IdentityDbContext<IdentityUser>
    {
        private DbContextOptions options;

        public AdminDbContext(DbContextOptions options)
        {
            this.options = options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUser>(b =>
            {
                b.ToTable("Admin_IdentityUser");
            });

            modelBuilder.Entity<RoleResource>(e =>
            {
                e.HasKey(k => new
                {
                    k.ResourceId,
                    k.RoleId
                });
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.ToTable("Admin_IdentityUserClaim");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.ToTable("Admin_IdentityUserLogin");
            });

            modelBuilder.Entity<IdentityUserToken<string>>(b =>
            {
                b.ToTable("Admin_IdentityUserToken");
            });

            modelBuilder.Entity<IdentityRole>(b =>
            {
                b.ToTable("Admin_IdentityRole");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.ToTable("Admin_IdentityRoleClaim");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(b =>
            {
                b.ToTable("Admin_IdentityUserRole");
            });
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
