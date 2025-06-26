using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Models.Entities.Base;

using static Models.Enums.User.RegistrationStatusEnum;

namespace Models.Entities.User
{
    [Index(nameof(Id), nameof(LanguageID))]
    public class Users : IdentityUser<long>/*, IBaseEntity<long>*/
    {
        [Column(TypeName = "NVARCHAR(50)", Order = 3)]
        public string FullName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(30)", Order = 4)]
        public string ICNumber { get; set; }

        [Column(TypeName = "nvarchar(500)", Order = 9)]
        public string? Salt { get; set; }

        [Column(TypeName = "nvarchar(500)", Order = 10)]
        public string? PINHash { get; set; }


        [Column(TypeName = "nvarchar(500)", Order = 11)]
        public string? MobileOTPHash { get; set; }
        public DateTime? MobileOTPSentAt { get; set; }


        [Column(TypeName = "nvarchar(500)", Order = 12)]
        public string? EmailOTPHash { get; set; }
        public DateTime? EmailOTPSentAt { get; set; }

        public RegistrationStatus Status { get; set; } = RegistrationStatus.PersonalInfo;

        [Column(Order = 6)]
        public int LanguageID { get; set; }

        public DateTime CreatedAt   {get; set; }
        public DateTime? UpdatedAt {get; set; }
        public string? CreatedBy    {get; set; }
        public string? UpdatedBy    {get; set; }
        public bool EnableBiometric { get; set; }
    }
}

