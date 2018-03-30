namespace Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Renaming_And_Author_Field : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Releases");
            AddColumn("dbo.Releases", "TimePublished", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Releases", "TimeFound", c => c.DateTime(nullable: false));
            AddColumn("dbo.Sources", "AuthorHolder", c => c.String());
            AddPrimaryKey("dbo.Releases", new[] { "Chapter", "ReleasableTitle", "SourceURL", "TimePublished" });
            DropColumn("dbo.Releases", "DatePublished");
            DropColumn("dbo.Releases", "Time");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Releases", "Time", c => c.DateTime(nullable: false));
            AddColumn("dbo.Releases", "DatePublished", c => c.String(nullable: false, maxLength: 128));
            DropPrimaryKey("dbo.Releases");
            DropColumn("dbo.Sources", "AuthorHolder");
            DropColumn("dbo.Releases", "TimeFound");
            DropColumn("dbo.Releases", "TimePublished");
            AddPrimaryKey("dbo.Releases", new[] { "Chapter", "ReleasableTitle", "SourceURL", "DatePublished" });
        }
    }
}
