using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_ASP_NET.Data;

namespace Project_ASP_NET.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider
       serviceProvider)
        {
            using (var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService
            <DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Roles.Any())
                {
                    return;
                }

                context.Roles.AddRange(
                new IdentityRole { Id = "cc538354-ad57-40f0-9adc-7923283b7853", Name = "Admin", NormalizedName = "Admin".ToUpper() },
                new IdentityRole { Id = "36896250-5000-422b-88e4-19a3884b1661", Name = "Mod", NormalizedName = "Mod".ToUpper() },
                new IdentityRole { Id = "347bfefd-62d2-4b5c-82f6-f12e29acb863", Name = "User", NormalizedName = "User".ToUpper() }
                );

                var hasher = new PasswordHasher<User>();

                context.Users.AddRange(
                new User
                {
                    Id = "80c87953-3d0b-46c6-942e-cbfd639290e4",
                    UserName = "admin@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@TEST.COM",
                    Email = "admin@test.com",
                    NormalizedUserName = "ADMIN@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Admin1!")
                },
               new User
               {
                   Id = "8af8a00a-e7f8-43e0-aec6-e14396163e6f",
                   UserName = "mod@test.com",
                   EmailConfirmed = true,
                   NormalizedEmail = "MOD@TEST.COM",
                   Email = "mod@test.com",
                   NormalizedUserName = "MOD@TEST.COM",
                   PasswordHash = hasher.HashPassword(null, "Moderator1!")
               },
                new User
                {
                    Id = "4938cc85-2445-461d-aa6a-6ade59927b8c",
                    UserName = "user@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "USER@TEST.COM",
                    Email = "user@test.com",
                    NormalizedUserName = "USER@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "User1!")
                }
);
                // ASOCIEREA USER-ROLE
                context.UserRoles.AddRange(
                new IdentityUserRole<string>
                {
                    RoleId = "cc538354-ad57-40f0-9adc-7923283b7853",
                    UserId = "80c87953-3d0b-46c6-942e-cbfd639290e4"
                },
               new IdentityUserRole<string>
               {
                   RoleId = "36896250-5000-422b-88e4-19a3884b1661",
                   UserId = "8af8a00a-e7f8-43e0-aec6-e14396163e6f"
               },
               new IdentityUserRole<string>
               {
                   RoleId = "347bfefd-62d2-4b5c-82f6-f12e29acb863",
                   UserId = "4938cc85-2445-461d-aa6a-6ade59927b8c"
               }
                );
                context.SaveChanges();
            }
        }
    }
}
