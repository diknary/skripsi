namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddServerList : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ServerLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IPAddress = c.String(nullable: false),
                        UserId = c.String(nullable: false),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ServerLists");
        }
    }
}
