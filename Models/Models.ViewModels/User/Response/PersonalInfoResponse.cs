using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Models.ViewModels.Localization.Response;
using Models.ViewModels.User.Response.Base;

namespace Models.ViewModels.User.Response
{

    public class PersonalInfoResponse : BaseResponse
    {
        public long UserID { get; set; }
    }
}
