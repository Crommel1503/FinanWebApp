namespace FinanWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedTableName : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.AspNetUsers", newName: "AspNetUsers");
            RenameColumn(table: "dbo.AspNetRoles", name: "AspNetRolesId", newName: "Id");
            RenameColumn(table: "dbo.AspNetUsers", name: "AspNetUsersId", newName: "Id");
            RenameColumn(table: "dbo.AspNetUserClaims", name: "AspNetUserClaimsId", newName: "Id");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.AspNetUserClaims", name: "Id", newName: "AspNetUserClaimsId");
            RenameColumn(table: "dbo.AspNetUsers", name: "Id", newName: "AspNetUsersId");
            RenameColumn(table: "dbo.AspNetRoles", name: "Id", newName: "AspNetRolesId");
            RenameTable(name: "dbo.AspNetUsers", newName: "AspNetUsers");
        }
    }
}
