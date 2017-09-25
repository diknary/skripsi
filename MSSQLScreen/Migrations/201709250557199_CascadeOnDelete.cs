namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CascadeOnDelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.JobLists", "AdminAccountId", "dbo.AdminAccounts");
            DropForeignKey("dbo.JobRunHistories", "AdminAccountId", "dbo.AdminAccounts");
            DropForeignKey("dbo.JobLists", "ServerListId", "dbo.ServerLists");
            DropForeignKey("dbo.JobRunHistories", "JobListId", "dbo.JobLists");
            DropIndex("dbo.JobLists", new[] { "ServerListId" });
            DropIndex("dbo.JobLists", new[] { "AdminAccountId" });
            DropIndex("dbo.JobRunHistories", new[] { "JobListId" });
            DropIndex("dbo.JobRunHistories", new[] { "AdminAccountId" });
            AlterColumn("dbo.JobLists", "ServerListId", c => c.Int(nullable: false));
            AlterColumn("dbo.JobRunHistories", "JobListId", c => c.Int(nullable: false));
            CreateIndex("dbo.JobLists", "ServerListId");
            CreateIndex("dbo.JobRunHistories", "JobListId");
            AddForeignKey("dbo.JobLists", "ServerListId", "dbo.ServerLists", "Id", cascadeDelete: true);
            AddForeignKey("dbo.JobRunHistories", "JobListId", "dbo.JobLists", "Id", cascadeDelete: true);
            DropColumn("dbo.JobLists", "AdminAccountId");
            DropColumn("dbo.JobRunHistories", "AdminAccountId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.JobRunHistories", "AdminAccountId", c => c.Int());
            AddColumn("dbo.JobLists", "AdminAccountId", c => c.Int());
            DropForeignKey("dbo.JobRunHistories", "JobListId", "dbo.JobLists");
            DropForeignKey("dbo.JobLists", "ServerListId", "dbo.ServerLists");
            DropIndex("dbo.JobRunHistories", new[] { "JobListId" });
            DropIndex("dbo.JobLists", new[] { "ServerListId" });
            AlterColumn("dbo.JobRunHistories", "JobListId", c => c.Int());
            AlterColumn("dbo.JobLists", "ServerListId", c => c.Int());
            CreateIndex("dbo.JobRunHistories", "AdminAccountId");
            CreateIndex("dbo.JobRunHistories", "JobListId");
            CreateIndex("dbo.JobLists", "AdminAccountId");
            CreateIndex("dbo.JobLists", "ServerListId");
            AddForeignKey("dbo.JobRunHistories", "JobListId", "dbo.JobLists", "Id");
            AddForeignKey("dbo.JobLists", "ServerListId", "dbo.ServerLists", "Id");
            AddForeignKey("dbo.JobRunHistories", "AdminAccountId", "dbo.AdminAccounts", "Id");
            AddForeignKey("dbo.JobLists", "AdminAccountId", "dbo.AdminAccounts", "Id");
        }
    }
}
