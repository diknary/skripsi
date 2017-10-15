namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveAdminId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ServerLists", "AdminAccountId", "dbo.AdminAccounts");
            DropIndex("dbo.ServerLists", new[] { "AdminAccountId" });
            DropColumn("dbo.ServerLists", "AdminAccountId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ServerLists", "AdminAccountId", c => c.Int());
            CreateIndex("dbo.ServerLists", "AdminAccountId");
            AddForeignKey("dbo.ServerLists", "AdminAccountId", "dbo.AdminAccounts", "Id", cascadeDelete: true);
        }
    }
}
