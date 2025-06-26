using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Models.Entities.Localization;

namespace Models.Entities.System
{
    [Table("System", Schema = "PolicyLocalization")]
    public class PolicyLocalization
    {
        [Key]
        public int ID { get; set; } = 1; // only one row is needed per language

        [Required]
        public int LanguageID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Content { get; set; }

        public Languages Language { get; set; }
    }
}
