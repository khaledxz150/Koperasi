using Core.Services.System;
using Core.UnitOfWork;

namespace Application.Services.System
{
    public class PolicyService : IPolicyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PolicyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GetPolicyContentAsync(int languageId)
        {
            return await _unitOfWork._policyLocalizationRepository.GetContentByLanguageIdAsync(languageId);
        }
    }
}
