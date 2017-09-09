namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SecondMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JobRunHistories", "JobListId", c => c.Int());
            CreateIndex("dbo.JobRunHistories", "JobListId");
            AddForeignKey("dbo.JobRunHistories", "JobListId", "dbo.JobLists", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JobRunHistories", "JobListId", "dbo.JobLists");
            DropIndex("dbo.JobRunHistories", new[] { "JobListId" });
            AlterColumn("dbo.JobRunHistories", "JobListId", c => c.Int());
        }
    }
}
