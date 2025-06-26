using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models.Entities.User;
using Models.Enums.User;

namespace Infrastructure.Data.Config
{
    public class User_Configuration : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            var salt = "static-salt"; // You may hash this dynamically if needed
            var userId = 1001L;

            builder.HasData(new Users
            {
                Id = userId,
                UserName = "MIGRATED1001",
                FullName = "Migrated User",
                Email = "migrated@example.com",
                PhoneNumber = "+966500000001",
                ICNumber = "MIG123456",
                LanguageID = 1,
                CreatedAt = DateTime.UtcNow,
                Salt = salt,
                Status = RegistrationStatusEnum.PersonalInfo// Start of registration
                // MobileOTPHash, EmailOTPHash, PINHash are null by design
            });
        }
    }

    public class IdentityUserLogin_Configuration : IEntityTypeConfiguration<IdentityUserLogin<long>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<long>> builder)
        {
            builder.HasKey(x => new { x.UserId });
        }
    }
}
