namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Relation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AdminLogs", "AdminAccountId", "dbo.AdminAccounts");
            DropIndex("dbo.AdminLogs", new[] { "AdminAccountId" });
            AlterColumn("dbo.AdminLogs", "AdminAccountId", c => c.Int(nullable: false));
            CreateIndex("dbo.AdminLogs", "AdminAccountId");
            AddForeignKey("dbo.AdminLogs", "AdminAccountId", "dbo.AdminAccounts", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdminLogs", "AdminAccountId", "dbo.AdminAccounts");
            DropIndex("dbo.AdminLogs", new[] { "AdminAccountId" });
            AlterColumn("dbo.AdminLogs", "AdminAccountId", c => c.Int());
            CreateIndex("dbo.AdminLogs", "AdminAccountId");
            AddForeignKey("dbo.AdminLogs", "AdminAccountId", "dbo.AdminAccounts", "Id");
        }
    }
}
