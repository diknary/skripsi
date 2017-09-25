namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FK : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JobRunHistories", "ResourceId", c => c.String());
            AddColumn("dbo.JobRunHistories", "AdminAccountId", c => c.Int());
            CreateIndex("dbo.JobRunHistories", "AdminAccountId");
            AddForeignKey("dbo.JobRunHistories", "AdminAccountId", "dbo.AdminAccounts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JobRunHistories", "AdminAccountId", "dbo.AdminAccounts");
            DropIndex("dbo.JobRunHistories", new[] { "AdminAccountId" });
            DropColumn("dbo.JobRunHistories", "AdminAccountId");
            DropColumn("dbo.JobRunHistories", "ResourceId");
        }
    }
}
