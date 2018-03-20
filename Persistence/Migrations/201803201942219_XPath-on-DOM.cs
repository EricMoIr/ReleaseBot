namespace Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class XPathonDOM : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DOMs", "XPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DOMs", "XPath");
        }
    }
}
