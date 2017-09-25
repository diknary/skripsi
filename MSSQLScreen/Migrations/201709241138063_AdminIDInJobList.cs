namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdminIDInJobList : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JobLists", "AdminAccountId", c => c.Int());
            CreateIndex("dbo.JobLists", "AdminAccountId");
            AddForeignKey("dbo.JobLists", "AdminAccountId", "dbo.AdminAccounts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JobLists", "AdminAccountId", "dbo.AdminAccounts");
            DropIndex("dbo.JobLists", new[] { "AdminAccountId" });
            DropColumn("dbo.JobLists", "AdminAccountId");
        }
    }
}
