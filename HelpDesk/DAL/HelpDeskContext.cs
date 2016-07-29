using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelpDesk.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HelpDesk.DAL
{
    public class HelpDeskContext : IdentityDbContext<ApplicationUser>
    {
        public HelpDeskContext() : base("HelpDeskContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Activity> Activities { get; set; }        
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<View> Views { get; set; }


        //public DbSet<ApplicationUser> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //this.OnModelCreating(modelBuilder);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
        }
    }
}