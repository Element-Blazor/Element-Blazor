using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazAdmin.Docs
{
    public class DocsDbContext : IdentityDbContext
    {
        public DocsDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
