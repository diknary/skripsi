namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTotalMemory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResourceUsages", "TotalMemory", c => c.Long(nullable: false));
            AlterColumn("dbo.ResourceUsages", "AvailableMemory", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ResourceUsages", "AvailableMemory", c => c.Int(nullable: false));
            DropColumn("dbo.ResourceUsages", "TotalMemory");
        }
    }
}
