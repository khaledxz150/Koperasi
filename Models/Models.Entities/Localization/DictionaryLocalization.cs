using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities.Localization
{
    public class DictionaryLocalization
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public int LanguageID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Description { get; set; }

        public Languages Language { get; set; }

        [ForeignKey(nameof(ID))]
        public Dictionary Dictionary { get; set; }
    }
}
