namespace FinanWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userRolesM : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUserRoles", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUserRoles", "Discriminator");
        }
    }
}
