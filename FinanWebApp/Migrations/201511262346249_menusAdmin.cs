namespace FinanWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class menusAdmin : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Menus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(nullable: true),
                        Name = c.String(),
                        ActionName = c.String(),
                        ControllerName = c.String(),
                        Url = c.String(),
                        RoleName = c.String(),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Menus");
        }
    }
}
