using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities.Localization
{
    public class Dictionary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public List<DictionaryLocalization> DictionaryLocalization { get; set; } = new List<DictionaryLocalization>();
    }
}
