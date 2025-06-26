using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.System
{
    public interface IPolicyService
    {
        Task<string> GetPolicyContentAsync(int languageId);
    }
}
