namespace Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IForgot : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Users", newName: "Subscribers");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Subscribers", newName: "Users");
        }
    }
}
