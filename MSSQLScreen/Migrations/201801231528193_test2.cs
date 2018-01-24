namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServerLists", "Salt", c => c.String());
            AddColumn("dbo.ResourceUsages", "CPUUsage", c => c.Int(nullable: false));
            AddColumn("dbo.ResourceUsages", "MemoryUsage", c => c.Long(nullable: false));
            DropColumn("dbo.ResourceUsages", "CPUBusy");
            DropColumn("dbo.ResourceUsages", "AvailableMemory");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ResourceUsages", "AvailableMemory", c => c.Long(nullable: false));
            AddColumn("dbo.ResourceUsages", "CPUBusy", c => c.Int(nullable: false));
            DropColumn("dbo.ResourceUsages", "MemoryUsage");
            DropColumn("dbo.ResourceUsages", "CPUUsage");
            DropColumn("dbo.ServerLists", "Salt");
        }
    }
}
