namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstMigration : DbMigration
    {
        public override void Up()
        {
          
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.LoginHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoginDate = c.DateTime(),
                        AdminID = c.Int(nullable: false),
                        AdminAccount_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AdminAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        NIP = c.String(),
                        Username = c.String(),
                        Password = c.String(),
                        Privilege = c.String(),
                        LastLogin = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.JobRunHistories", "JobActivityID", c => c.Int());
            RenameIndex(table: "dbo.JobRunHistories", name: "IX_JobListId", newName: "IX_JobList_Id");
            RenameColumn(table: "dbo.JobRunHistories", name: "JobListId", newName: "JobList_Id");
        }
    }
}
