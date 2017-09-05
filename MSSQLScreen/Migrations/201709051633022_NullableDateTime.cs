namespace MSSQLScreen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullableDateTime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserAccounts", "LastLogin", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserAccounts", "LastLogin", c => c.DateTime(nullable: false));
        }
    }
}
