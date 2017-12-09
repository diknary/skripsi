namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsOnline : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdminAccounts", "IsOnline", c => c.Byte(nullable: false));
            DropColumn("dbo.AdminAccounts", "IsConnected");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AdminAccounts", "IsConnected", c => c.Byte(nullable: false));
            DropColumn("dbo.AdminAccounts", "IsOnline");
        }
    }
}
