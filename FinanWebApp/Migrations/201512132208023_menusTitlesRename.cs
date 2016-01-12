namespace FinanWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class menusTitlesRename : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Menus", "Title", c => c.String());
            DropColumn("dbo.Menus", "Titulo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Menus", "Titulo", c => c.String());
            DropColumn("dbo.Menus", "Title");
        }
    }
}
