namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeDayaType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ResourceUsages", "ProcessorUsage", c => c.Int(nullable: false));
            AlterColumn("dbo.ResourceUsages", "AvailableMemory", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ResourceUsages", "AvailableMemory", c => c.Single(nullable: false));
            AlterColumn("dbo.ResourceUsages", "ProcessorUsage", c => c.Single(nullable: false));
        }
    }
}
