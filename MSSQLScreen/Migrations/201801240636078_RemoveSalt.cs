namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveSalt : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ServerLists", "Salt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ServerLists", "Salt", c => c.String());
        }
    }
}
