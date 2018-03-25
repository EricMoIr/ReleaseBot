namespace Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removed_DOM : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Sources", "ChapterNumberHolder_Id", "dbo.DOMs");
            DropForeignKey("dbo.Sources", "DateTimeHolder_Id", "dbo.DOMs");
            DropForeignKey("dbo.Sources", "ReleaseHolder_Id", "dbo.DOMs");
            DropIndex("dbo.Sources", new[] { "ChapterNumberHolder_Id" });
            DropIndex("dbo.Sources", new[] { "DateTimeHolder_Id" });
            DropIndex("dbo.Sources", new[] { "ReleaseHolder_Id" });
            AddColumn("dbo.Sources", "ReleaseHolder", c => c.String());
            AddColumn("dbo.Sources", "ChapterNumberHolder", c => c.String());
            AddColumn("dbo.Sources", "DateTimeHolder", c => c.String());
            DropColumn("dbo.Sources", "ChapterNumberHolder_Id");
            DropColumn("dbo.Sources", "DateTimeHolder_Id");
            DropColumn("dbo.Sources", "ReleaseHolder_Id");
            DropTable("dbo.DOMs");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DOMs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClassAttribute = c.String(),
                        IdAttribute = c.String(),
                        Tag = c.String(),
                        ParentClassAttribute = c.String(),
                        ChildPosition = c.Int(nullable: false),
                        ParentTag = c.String(),
                        XPath = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Sources", "ReleaseHolder_Id", c => c.Int());
            AddColumn("dbo.Sources", "DateTimeHolder_Id", c => c.Int());
            AddColumn("dbo.Sources", "ChapterNumberHolder_Id", c => c.Int());
            DropColumn("dbo.Sources", "DateTimeHolder");
            DropColumn("dbo.Sources", "ChapterNumberHolder");
            DropColumn("dbo.Sources", "ReleaseHolder");
            CreateIndex("dbo.Sources", "ReleaseHolder_Id");
            CreateIndex("dbo.Sources", "DateTimeHolder_Id");
            CreateIndex("dbo.Sources", "ChapterNumberHolder_Id");
            AddForeignKey("dbo.Sources", "ReleaseHolder_Id", "dbo.DOMs", "Id");
            AddForeignKey("dbo.Sources", "DateTimeHolder_Id", "dbo.DOMs", "Id");
            AddForeignKey("dbo.Sources", "ChapterNumberHolder_Id", "dbo.DOMs", "Id");
        }
    }
}
