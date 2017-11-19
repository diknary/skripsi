namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoginHistoriesToAdminLog : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.LoginHistories", newName: "AdminLogs");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.AdminLogs", newName: "LoginHistories");
        }
    }
}
