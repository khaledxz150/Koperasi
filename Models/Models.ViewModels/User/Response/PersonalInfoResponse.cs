using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Models.Enums.System;
using Models.ViewModels.Localization.Response;
using Models.ViewModels.User.Response.Base;

namespace Models.ViewModels.User.Response
{

    public class PersonalInfoResponse : BaseResponse
    {
        public string UserCode { get; set; }
        public string NextStep { get; set; }
        public ValidationPopupTypesEnum ValidationPopupType { get; set; }
    }
}
