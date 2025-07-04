﻿using Application.UnitOfWork.Repos;

using Core.UnitOfWork.Repos;

namespace Core.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();
        int SaveChanges();
        // Add repository properties here, e.g.:
        IUserRepository _userRepository { get; }
        ILanguagesRepository __languagesRepository { get; }
        IDictionaryLocalizationRepository _dictionaryLocalizationRepository { get; }
        IPolicyLocalizationRepository _policyLocalizationRepository { get; }
    }
}
