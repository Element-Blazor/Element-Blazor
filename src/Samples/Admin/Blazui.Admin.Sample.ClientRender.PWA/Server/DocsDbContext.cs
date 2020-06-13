using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blazui.Admin.Sample.ClientRender.PWA.Server
{
    public class DocsDbContext : IdentityDbContext
    {
        public DocsDbContext(DbContextOptions<DocsDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<RoleResource>(e =>
            {
                e.HasKey(x => new
                {
                    x.ResourceId,
                    x.RoleId
                });
            });
        }

        public DbSet<RoleResource> RoleResources { get; set; }
    }
}
