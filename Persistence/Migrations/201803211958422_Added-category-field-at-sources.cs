namespace Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedcategoryfieldatsources : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Releasables", "Category", c => c.String());
            AddColumn("dbo.Sources", "Category", c => c.String());
            DropColumn("dbo.Releasables", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Releasables", "Type", c => c.String());
            DropColumn("dbo.Sources", "Category");
            DropColumn("dbo.Releasables", "Category");
        }
    }
}
