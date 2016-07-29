namespace HelpDesk.Migrations
{
    using HelpDesk.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<HelpDesk.DAL.HelpDeskContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(HelpDesk.DAL.HelpDeskContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            //context.Products.AddOrUpdate(
            //        p => p.Name,
            //        new Product { Id = 1, Name = "Ascentis" },
            //        new Product { Id = 2, Name = "Apex" }
            //    );
            //context.Modules.AddOrUpdate(
            //        m => m.Name,
            //        new Module { Id = 1, Name = "Module One", ProductId = 1}
            //    );
            //context.Activities.AddOrUpdate(
            //        a => a.Name,
            //        new Activity {  Id = 1, Name = "Activiet 1", Description = "Descriotujsdf sdfljdoiagd ", ModuleId = 1}
            //    );

            //context.Activities.AddOrUpdate(
            //        a => a.Name,
            //        new Activity { Id = 2, Name = "Activiet 2", Description = "Descriotujsdf sdfljdoiagd ", ModuleId = 1 }
            //    );
            //context.Steps.AddOrUpdate(
            //        s => s.Name,
            //        new Step { Id = 1, Name = "Step 1", Description = "Descriotujsdf sdfljdoiagd ", ActivityId = 1, Order = 1 }
            //    );
            //context.Steps.AddOrUpdate(
            //        s => s.Name,
            //        new Step { Id = 2, Name = "Step 2", Description = "Descriotujsdf sdfljdoiagd ", ActivityId = 1, Order = 2 }
            //    );
            //context.Steps.AddOrUpdate(
            //        s => s.Name,
            //        new Step { Id = 3, Name = "Step 4", Description = "Descriotujsdf sdfljdoiagd ", ActivityId = 1, Order = 3 }
            //    );

            //context.Steps.AddOrUpdate(
            //        s => s.Name,
            //        new Step { Id = 4, Name = "Step 1", Description = "Descriotujsdf sdfljdoiagd ", ActivityId = 2, Order = 1 }
            //    );
            //context.Steps.AddOrUpdate(
            //        s => s.Name,
            //        new Step { Id = 5, Name = "Step 2", Description = "Descriotujsdf sdfljdoiagd ", ActivityId = 2, Order = 2 }
            //    );
            //context.Steps.AddOrUpdate(
            //        s => s.Name,
            //        new Step { Id = 6, Name = "Step 3", Description = "Descriotujsdf sdfljdoiagd ", ActivityId = 2, Order = 3}
            //    );


            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>());
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!roleManager.RoleExists(RoleNames.ROLE_ADMIN))
            {
                var roleresult = roleManager.Create(new IdentityRole(RoleNames.ROLE_ADMIN));

            }

            var newUser = new ApplicationUser();
            newUser.UserName = "admin@extensisgroup.com";
            newUser.FirstName = "Admin";
            newUser.LastName = "User";

            var createUserResult =  userManager.Create(newUser, "secret");
            if (createUserResult.Succeeded){
            
                userManager.AddToRole(newUser.Id, RoleNames.ROLE_ADMIN);
            }
            
        }
    }
}
