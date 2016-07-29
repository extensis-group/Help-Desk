namespace HelpDesk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(nullable: false, maxLength: 250),
                        ModuleId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.ModuleId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Modules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Steps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(nullable: false),
                        Order = c.Int(nullable: false),
                        ActivityId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Activities", t => t.ActivityId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.ActivityId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Issues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Description = c.String(nullable: false),
                        WasSeenByAdmin = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        StepId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        UserDTO_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.Steps", t => t.StepId, cascadeDelete: true)
                .ForeignKey("dbo.UserDTOes", t => t.UserDTO_Id)
                .Index(t => t.StepId)
                .Index(t => t.UserId)
                .Index(t => t.UserDTO_Id);
            
            CreateTable(
                "dbo.Solutions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Description = c.String(nullable: false, maxLength: 500),
                        IsCorrect = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        IssueId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Issues", t => t.IssueId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.IssueId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.UserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name)
                .Index(t => t.Name, unique: true, name: "NameIndex");
            
            CreateTable(
                "dbo.UserDTOes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        UserName = c.String(),
                        IsAuthenticated = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Views",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ViewedAt = c.DateTime(nullable: false),
                        UserId = c.String(maxLength: 128),
                        ActivityId = c.Int(),
                        IssueId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Activities", t => t.ActivityId)
                .ForeignKey("dbo.Issues", t => t.IssueId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ActivityId)
                .Index(t => t.IssueId);
            
            CreateTable(
                "dbo.TagActivities",
                c => new
                    {
                        Tag_Id = c.Int(nullable: false),
                        Activity_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Activity_Id })
                .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)
                .ForeignKey("dbo.Activities", t => t.Activity_Id, cascadeDelete: true)
                .Index(t => t.Tag_Id)
                .Index(t => t.Activity_Id);
            
            CreateTable(
                "dbo.TagIssues",
                c => new
                    {
                        Tag_Id = c.Int(nullable: false),
                        Issue_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Issue_Id })
                .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)
                .ForeignKey("dbo.Issues", t => t.Issue_Id, cascadeDelete: true)
                .Index(t => t.Tag_Id)
                .Index(t => t.Issue_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        IsAuthenticated = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Views", "UserId", "dbo.Users");
            DropForeignKey("dbo.Views", "IssueId", "dbo.Issues");
            DropForeignKey("dbo.Views", "ActivityId", "dbo.Activities");
            DropForeignKey("dbo.Issues", "UserDTO_Id", "dbo.UserDTOes");
            DropForeignKey("dbo.TagIssues", "Issue_Id", "dbo.Issues");
            DropForeignKey("dbo.TagIssues", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.TagActivities", "Activity_Id", "dbo.Activities");
            DropForeignKey("dbo.TagActivities", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.Issues", "StepId", "dbo.Steps");
            DropForeignKey("dbo.Steps", "UserId", "dbo.Users");
            DropForeignKey("dbo.Solutions", "UserId", "dbo.Users");
            DropForeignKey("dbo.Issues", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserClaims", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.UserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Activities", "UserId", "dbo.Users");
            DropForeignKey("dbo.Solutions", "IssueId", "dbo.Issues");
            DropForeignKey("dbo.Steps", "ActivityId", "dbo.Activities");
            DropForeignKey("dbo.Modules", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Activities", "ModuleId", "dbo.Modules");
            DropIndex("dbo.Users", new[] { "Id" });
            DropIndex("dbo.TagIssues", new[] { "Issue_Id" });
            DropIndex("dbo.TagIssues", new[] { "Tag_Id" });
            DropIndex("dbo.TagActivities", new[] { "Activity_Id" });
            DropIndex("dbo.TagActivities", new[] { "Tag_Id" });
            DropIndex("dbo.Views", new[] { "IssueId" });
            DropIndex("dbo.Views", new[] { "ActivityId" });
            DropIndex("dbo.Views", new[] { "UserId" });
            DropIndex("dbo.Tags", "NameIndex");
            DropIndex("dbo.Tags", new[] { "Name" });
            DropIndex("dbo.UserRoles", new[] { "RoleId" });
            DropIndex("dbo.UserRoles", new[] { "UserId" });
            DropIndex("dbo.UserLogins", new[] { "UserId" });
            DropIndex("dbo.UserClaims", new[] { "User_Id" });
            DropIndex("dbo.Solutions", new[] { "UserId" });
            DropIndex("dbo.Solutions", new[] { "IssueId" });
            DropIndex("dbo.Issues", new[] { "UserDTO_Id" });
            DropIndex("dbo.Issues", new[] { "UserId" });
            DropIndex("dbo.Issues", new[] { "StepId" });
            DropIndex("dbo.Steps", new[] { "UserId" });
            DropIndex("dbo.Steps", new[] { "ActivityId" });
            DropIndex("dbo.Modules", new[] { "ProductId" });
            DropIndex("dbo.Activities", new[] { "UserId" });
            DropIndex("dbo.Activities", new[] { "ModuleId" });
            DropTable("dbo.Users");
            DropTable("dbo.TagIssues");
            DropTable("dbo.TagActivities");
            DropTable("dbo.Views");
            DropTable("dbo.UserDTOes");
            DropTable("dbo.Tags");
            DropTable("dbo.Roles");
            DropTable("dbo.UserRoles");
            DropTable("dbo.UserLogins");
            DropTable("dbo.UserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Solutions");
            DropTable("dbo.Issues");
            DropTable("dbo.Steps");
            DropTable("dbo.Products");
            DropTable("dbo.Modules");
            DropTable("dbo.Activities");
        }
    }
}
