namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DiskUsageRename : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DiskUsages", "AvailabeSpace", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DiskUsages", "AvailabeSpace", c => c.String());
        }
    }
}
