namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoginHistoryAndAdmin : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.JobRunHistories", "JobActivityId", "dbo.JobLists");
            DropIndex("dbo.JobRunHistories", new[] { "JobActivityId" });
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
            
            CreateTable(
                "dbo.LoginHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoginDate = c.DateTime(),
                        AdminID = c.Int(nullable: false),
                        AdminAccount_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdminAccounts", t => t.AdminAccount_Id)
                .Index(t => t.AdminAccount_Id);
            
            AddColumn("dbo.JobRunHistories", "JobList_Id", c => c.Int());
            CreateIndex("dbo.JobRunHistories", "JobList_Id");
            AddForeignKey("dbo.JobRunHistories", "JobList_Id", "dbo.JobLists", "Id");
            DropColumn("dbo.UserAccounts", "Privilege");
            DropColumn("dbo.UserAccounts", "LastLogin");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserAccounts", "LastLogin", c => c.DateTime());
            AddColumn("dbo.UserAccounts", "Privilege", c => c.String());
            DropForeignKey("dbo.JobRunHistories", "JobList_Id", "dbo.JobLists");
            DropForeignKey("dbo.LoginHistories", "AdminAccount_Id", "dbo.AdminAccounts");
            DropIndex("dbo.LoginHistories", new[] { "AdminAccount_Id" });
            DropIndex("dbo.JobRunHistories", new[] { "JobList_Id" });
            DropColumn("dbo.JobRunHistories", "JobList_Id");
            DropTable("dbo.LoginHistories");
            DropTable("dbo.AdminAccounts");
            CreateIndex("dbo.JobRunHistories", "JobActivityId");
            AddForeignKey("dbo.JobRunHistories", "JobActivityId", "dbo.JobLists", "Id");
        }
    }
}
