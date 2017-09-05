namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLastLogin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserAccounts", "LastLogin", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserAccounts", "LastLogin");
        }
    }
}
