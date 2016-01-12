namespace FinanWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class menusTitles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Menus", "Titulo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Menus", "Titulo");
        }
    }
}
