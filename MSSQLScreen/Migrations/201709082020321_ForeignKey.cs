namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignKey : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.JobRunHistories", name: "JobList_Id", newName: "JobListID");
            RenameColumn(table: "dbo.LoginHistories", name: "AdminAccount_Id", newName: "AdminAccountID");
            RenameIndex(table: "dbo.JobRunHistories", name: "IX_JobList_Id", newName: "IX_JobListID");
            RenameIndex(table: "dbo.LoginHistories", name: "IX_AdminAccount_Id", newName: "IX_AdminAccountID");
            DropColumn("dbo.JobRunHistories", "JobActivityID");
            DropColumn("dbo.LoginHistories", "AdminID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LoginHistories", "AdminID", c => c.Int(nullable: false));
            AddColumn("dbo.JobRunHistories", "JobActivityID", c => c.Int());
            RenameIndex(table: "dbo.LoginHistories", name: "IX_AdminAccountID", newName: "IX_AdminAccount_Id");
            RenameIndex(table: "dbo.JobRunHistories", name: "IX_JobListID", newName: "IX_JobList_Id");
            RenameColumn(table: "dbo.LoginHistories", name: "AdminAccountID", newName: "AdminAccount_Id");
            RenameColumn(table: "dbo.JobRunHistories", name: "JobListID", newName: "JobList_Id");
        }
    }
}
