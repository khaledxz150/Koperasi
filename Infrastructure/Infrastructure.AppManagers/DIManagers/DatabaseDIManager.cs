using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Koperasi.Managers
{
    public static class DatabaseDIManager
    {
        public static void InjectDB<T, User>(this IServiceCollection Services, string CS, bool EnableSensitiveDataLogging = false) where T : IdentityDbContext<User, IdentityRole<long>, long>
              where User : IdentityUser<long>
        {
            Services.AddDbContext<T>(options =>
            {
                options.UseSqlServer(CS);
                if (EnableSensitiveDataLogging)
                {
                    options.EnableSensitiveDataLogging();
                }
            });

        }

        public static void InjectIdentity<DbContext,User>(this IServiceCollection services, IConfiguration configuration) where DbContext : IdentityDbContext<User, IdentityRole<long>, long>
            where User : IdentityUser<long>
        {
            // Configure Identity
            services.AddIdentity<User, IdentityRole<long>>(options =>
            {
                // Password requirements
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<DbContext>()
            .AddDefaultTokenProviders();

           

           
        }

    }
}
