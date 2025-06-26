using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities.Localization
{
    [Table("Dictionary", Schema ="Localization")]
    public class Dictionary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public List<DictionaryLocalization> DictionaryLocalization { get; set; } = new List<DictionaryLocalization>();
    }
}
