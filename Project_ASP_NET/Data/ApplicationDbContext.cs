using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Project_ASP_NET.Models;
using Group = Project_ASP_NET.Models.Group;

namespace Project_ASP_NET.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Membership> Memberships { get; set; }

        public DbSet<MemberRequest> MemberRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Membership>()
                .HasKey(ms => new { ms.Id, ms.UserId, ms.GroupId });

            modelBuilder.Entity<Membership>()
                .HasOne(ms => ms.User)
                .WithMany(u => u.Memberships)
                .HasForeignKey(ms => ms.UserId);

            modelBuilder.Entity<Membership>()
                .HasOne(ms => ms.Group)
                .WithMany(g => g.Memberships)
                .HasForeignKey(ms => ms.GroupId);

            modelBuilder.Entity<MemberRequest>()
                .HasKey(ms => new { ms.Id, ms.UserId, ms.GroupId });

            modelBuilder.Entity<MemberRequest>()
                .HasOne(ms => ms.User)
                .WithMany(u => u.MemberRequests)
                .HasForeignKey(ms => ms.UserId);

            modelBuilder.Entity<MemberRequest>()
                .HasOne(ms => ms.Group)
                .WithMany(g => g.MemberRequests)
                .HasForeignKey(ms => ms.GroupId);
        }

       
    }
}