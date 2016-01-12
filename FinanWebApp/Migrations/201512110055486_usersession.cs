namespace FinanWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usersession : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserSessions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        AccessDate = c.DateTime(nullable: false),
                        IsOnLine = c.Boolean(nullable: false),
                        IpAddress = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserSessions");
        }
    }
}
