namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDuration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JobLists", "Duration", c => c.Int(nullable: false));
            AlterColumn("dbo.JobDetails", "Duration", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.JobDetails", "Duration", c => c.String());
            DropColumn("dbo.JobLists", "Duration");
        }
    }
}
