namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoginHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LoginHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoginDate = c.DateTime(),
                        AdminAccountId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdminAccounts", t => t.AdminAccountId)
                .Index(t => t.AdminAccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LoginHistories", "AdminAccountId", "dbo.AdminAccounts");
            DropIndex("dbo.LoginHistories", new[] { "AdminAccountId" });
            DropTable("dbo.LoginHistories");
        }
    }
}
