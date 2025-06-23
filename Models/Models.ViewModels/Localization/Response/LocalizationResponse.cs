using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels.Localization.Response
{
    public class LocalizationResponse
    {
        public string LanguageName { get; set; }
        public string Direction { get; set; }
        public string DateFormat { get; set; }
        public string NumberFormat { get; set; }
        public Dictionary<int, string> Words { get; set; }
    }
}
