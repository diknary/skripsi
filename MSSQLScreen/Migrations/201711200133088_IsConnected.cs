namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsConnected : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdminAccounts", "IsConnected", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AdminAccounts", "IsConnected");
        }
    }
}
