namespace HelpDesk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Issues", "UserDTO_Id", "dbo.UserDTOes");
            DropIndex("dbo.Issues", new[] { "UserDTO_Id" });
            DropColumn("dbo.Issues", "UserDTO_Id");
            DropTable("dbo.UserDTOes");
        }
        
        public override void Down()
        {
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
            
            AddColumn("dbo.Issues", "UserDTO_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Issues", "UserDTO_Id");
            AddForeignKey("dbo.Issues", "UserDTO_Id", "dbo.UserDTOes", "Id");
        }
    }
}
