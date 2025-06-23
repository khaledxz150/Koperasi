

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Koperasi.Managers
{
    public static class DatabaseDIManager
    {
        public static void InjectDB<T>(this IServiceCollection Services, string CS, bool EnableSensitiveDataLogging = false) where T : DbContext
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
    }
}
