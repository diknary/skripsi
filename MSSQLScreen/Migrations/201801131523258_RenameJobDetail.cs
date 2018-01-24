namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameJobDetail : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.JobDetails", newName: "JobHistories");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.JobHistories", newName: "JobDetails");
        }
    }
}
