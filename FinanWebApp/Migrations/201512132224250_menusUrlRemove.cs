namespace FinanWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class menusUrlRemove : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Menus", "Url");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Menus", "Url", c => c.String());
        }
    }
}
