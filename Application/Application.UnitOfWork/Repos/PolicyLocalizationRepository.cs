using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.UnitOfWork.Repos;

using Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

using Models.Entities.System;

namespace Application.UnitOfWork.Repos
{
    public class PolicyLocalizationRepository : GenericRepository<PolicyLocalization>, IPolicyLocalizationRepository
    {
        public PolicyLocalizationRepository(ApplicationDbContext context) : base(context) { }

        public async Task<string> GetContentByLanguageIdAsync(int languageId)
        {
            return (await _context.PolicyLocalizations
                .FirstOrDefaultAsync(x => x.LanguageID == languageId))?.Content
                ?? "Policy not available in this language.";
        }
    }
}
