namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removeisconnected : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AdminAccounts", "IsConnected");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AdminAccounts", "IsConnected", c => c.Boolean(nullable: false));
        }
    }
}
