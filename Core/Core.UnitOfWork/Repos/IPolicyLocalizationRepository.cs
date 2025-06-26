using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Models.Entities.System;

namespace Core.UnitOfWork.Repos
{
    public interface IPolicyLocalizationRepository : IGenericRepository<PolicyLocalization>
    {
        Task<string> GetContentByLanguageIdAsync(int languageId);
    }
}
