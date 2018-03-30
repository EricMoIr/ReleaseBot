namespace Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Author_Field : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Releases", "Author", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Releases", "Author");
        }
    }
}
