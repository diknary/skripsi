namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditUserFlow : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdminAccounts", "UserAccountId", c => c.Int(nullable: false));
            AddColumn("dbo.UserAccounts", "Salt", c => c.String());
            CreateIndex("dbo.AdminAccounts", "UserAccountId");
            AddForeignKey("dbo.AdminAccounts", "UserAccountId", "dbo.UserAccounts", "Id", cascadeDelete: true);
            DropColumn("dbo.AdminAccounts", "Name");
            DropColumn("dbo.AdminAccounts", "NIP");
            DropColumn("dbo.AdminAccounts", "Username");
            DropColumn("dbo.AdminAccounts", "Password");
            DropColumn("dbo.UserAccounts", "NIP");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserAccounts", "NIP", c => c.String());
            AddColumn("dbo.AdminAccounts", "Password", c => c.String());
            AddColumn("dbo.AdminAccounts", "Username", c => c.String());
            AddColumn("dbo.AdminAccounts", "NIP", c => c.String());
            AddColumn("dbo.AdminAccounts", "Name", c => c.String());
            DropForeignKey("dbo.AdminAccounts", "UserAccountId", "dbo.UserAccounts");
            DropIndex("dbo.AdminAccounts", new[] { "UserAccountId" });
            DropColumn("dbo.UserAccounts", "Salt");
            DropColumn("dbo.AdminAccounts", "UserAccountId");
        }
    }
}
