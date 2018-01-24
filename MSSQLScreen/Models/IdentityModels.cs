using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MSSQLScreen.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<JobList> JobLists { get; set; }
        public DbSet<JobHistory> JobDetails { get; set; }
        public DbSet<ResourceUsage> ResourceUsages { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<AdminAccount> AdminAccounts { get; set; }
        public DbSet<AdminLog> AdminLogs { get; set; }
        public DbSet<ServerList> ServerLists { get; set; }
        public DbSet<DiskUsage> DiskUsages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiskUsage>()
                .HasRequired(c => c.ServerList)
                .WithMany()
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<ResourceUsage>()
                .HasRequired(c => c.ServerList)
                .WithMany()
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<JobList>()
                .HasRequired(c => c.ServerList)
                .WithMany()
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<JobHistory>()
                .HasRequired(c => c.JobList)
                .WithMany()
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<AdminAccount>()
                .HasRequired(c => c.UserAccount)
                .WithMany()
                .WillCascadeOnDelete(true);
            base.OnModelCreating(modelBuilder);
        }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}