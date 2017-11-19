namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JobRunHistoryToJobDetail : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.JobRunHistories", newName: "JobDetails");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.JobDetails", newName: "JobRunHistories");
        }
    }
}
