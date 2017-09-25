namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CascadeOnDeleteServerList : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ServerLists", "AdminAccountId", "dbo.AdminAccounts");
            AddForeignKey("dbo.ServerLists", "AdminAccountId", "dbo.AdminAccounts", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServerLists", "AdminAccountId", "dbo.AdminAccounts");
            AddForeignKey("dbo.ServerLists", "AdminAccountId", "dbo.AdminAccounts", "Id");
        }
    }
}
