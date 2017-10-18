namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServerIdinResource : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResourceUsages", "ServerListId", c => c.Int(nullable: false));
            CreateIndex("dbo.ResourceUsages", "ServerListId");
            AddForeignKey("dbo.ResourceUsages", "ServerListId", "dbo.ServerLists", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ResourceUsages", "ServerListId", "dbo.ServerLists");
            DropIndex("dbo.ResourceUsages", new[] { "ServerListId" });
            DropColumn("dbo.ResourceUsages", "ServerListId");
        }
    }
}
