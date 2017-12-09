namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cpuBusy : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResourceUsages", "CPUBusy", c => c.Int(nullable: false));
            DropColumn("dbo.ResourceUsages", "ProcessorUsage");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ResourceUsages", "ProcessorUsage", c => c.Int(nullable: false));
            DropColumn("dbo.ResourceUsages", "CPUBusy");
        }
    }
}
