using Application.UnitOfWork.Repos;

using Core.UnitOfWork;
using Core.UnitOfWork.Repos;

using Infrastructure.Data;

namespace Application.UnitOfWork
{
   

    public class UnitOfWork : IUnitOfWork 
    {
        private readonly ApplicationDbContext _context;
        private bool _disposed = false;

        private IUserRepository? _userRepositoryInstance;
        private ILanguagesRepository? _languagesRepositoryInstance;
        private IDictionaryLocalizationRepository? _dictionaryLocalizationRepositoryInstance;
        private IPolicyLocalizationRepository? _policyLocalizationRepositoryInstance;

        public IUserRepository _userRepository
        {
            get
            {
                if (_userRepositoryInstance == null)
                {
                    _userRepositoryInstance = new UserRepository(_context);
                }
                return _userRepositoryInstance;
            }
        }

        public ILanguagesRepository __languagesRepository
        {
            get
            {
                if (_languagesRepositoryInstance == null)
                {
                    _languagesRepositoryInstance = new LanguagesRepository(_context);
                }
                return _languagesRepositoryInstance;
            }
        }

        public IDictionaryLocalizationRepository _dictionaryLocalizationRepository
        {
            get
            {
                if (_dictionaryLocalizationRepositoryInstance == null)
                {
                    _dictionaryLocalizationRepositoryInstance = new DictionaryLocalizationRepository(_context);
                }
                return _dictionaryLocalizationRepositoryInstance;
            }
        }


        public IPolicyLocalizationRepository _policyLocalizationRepository
        {
            get
            {
                if (_policyLocalizationRepositoryInstance == null)
                {
                    _policyLocalizationRepositoryInstance = new PolicyLocalizationRepository(_context);
                }
                return _policyLocalizationRepositoryInstance;
            }
        }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
