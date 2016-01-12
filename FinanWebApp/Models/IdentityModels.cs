using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FinanWebApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class User : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class Roles : IdentityRole
    {

    }

    public class UserRoles : IdentityUserRole
    {
        
    }

    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("AspNetUsers").Property(p => p.Id).HasColumnName("Id");
            modelBuilder.Entity<IdentityUserRole>().ToTable("AspNetUserRoles");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("AspNetUserLogins");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("AspNetUserClaims").Property(p => p.Id).HasColumnName("Id"); ;
            modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles").Property(p => p.Id).HasColumnName("Id");
        }

        public System.Data.Entity.DbSet<FinanWebApp.Models.Roles> IdentityRoles { get; set; }

        public System.Data.Entity.DbSet<FinanWebApp.Models.UserRoles> IdentityUserRoles { get; set; }

        public System.Data.Entity.DbSet<FinanWebApp.Models.Menus> Menus { get; set; }

        public System.Data.Entity.DbSet<FinanWebApp.Models.UserSession> UserSessions { get; set; }
    }
}