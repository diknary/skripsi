namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveResourceId : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.JobDetails", "ResourceId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.JobDetails", "ResourceId", c => c.String());
        }
    }
}
