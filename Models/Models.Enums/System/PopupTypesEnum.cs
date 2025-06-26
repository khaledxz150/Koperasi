using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Enums.System
{
    public enum ValidationPopupTypesEnum
    {
        None = 0,
        SuggestLogin = 1,
        SuggestLoginAndRetry = 2,
        SuggestRegister = 3,
        SuggestEntryRetry = 4
    }
}
