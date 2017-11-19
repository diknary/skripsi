namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveIsConnected : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdminAccounts", "IsConnected", c => c.Boolean(nullable: false));
            DropColumn("dbo.AdminLogs", "IsConnected");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AdminLogs", "IsConnected", c => c.Boolean(nullable: false));
            DropColumn("dbo.AdminAccounts", "IsConnected");
        }
    }
}
