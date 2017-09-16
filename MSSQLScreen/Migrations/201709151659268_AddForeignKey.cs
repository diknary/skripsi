namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServerLists", "AdminAccountId", c => c.Int());
            CreateIndex("dbo.ServerLists", "AdminAccountId");
            AddForeignKey("dbo.ServerLists", "AdminAccountId", "dbo.AdminAccounts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServerLists", "AdminAccountId", "dbo.AdminAccounts");
            DropIndex("dbo.ServerLists", new[] { "AdminAccountId" });
            DropColumn("dbo.ServerLists", "AdminAccountId");
        }
    }
}
