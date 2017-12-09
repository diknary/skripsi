namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirebaseTokenandStatusServer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdminAccounts", "FirebaseToken", c => c.String());
            AddColumn("dbo.ServerLists", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServerLists", "IsActive");
            DropColumn("dbo.AdminAccounts", "FirebaseToken");
        }
    }
}
