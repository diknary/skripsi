namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsConnected : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdminLogs", "IsConnected", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AdminLogs", "IsConnected");
        }
    }
}
