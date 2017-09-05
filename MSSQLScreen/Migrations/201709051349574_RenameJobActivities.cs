namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameJobActivities : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.JobActivities", newName: "JobLists");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.JobLists", newName: "JobActivities");
        }
    }
}
