using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities.Localization
{
    public class Languages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        [Required]
        public string LanguageName { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        [Required]
        public string Description { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        [Required]
        public string Direction { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        [Required]
        public string Culture { get; set; } 
    }
}
