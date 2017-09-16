namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignKeyJobList : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JobLists", "ServerListId", c => c.Int());
            CreateIndex("dbo.JobLists", "ServerListId");
            AddForeignKey("dbo.JobLists", "ServerListId", "dbo.ServerLists", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JobLists", "ServerListId", "dbo.ServerLists");
            DropIndex("dbo.JobLists", new[] { "ServerListId" });
            DropColumn("dbo.JobLists", "ServerListId");
        }
    }
}
