using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Seagull.Core;
using Seagull.Core.Data;
using Seagull.Core.Domain.Users;
using Seagull.Core.Domain.Localization;
using Seagull.Core.Domain.Stores;
using Seagull.Core.Infrastructure;
using Seagull.Data;
using Seagull.Services.Users;
using Seagull.Services.Localization;

namespace Seagull.Services.Installation
{
    public partial class SqlFileInstallationService : IInstallationService
    {
        #region Fields

        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IDbContext _dbContext;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public SqlFileInstallationService(IRepository<Language> languageRepository,
            IRepository<User> userRepository,
            IRepository<Store> storeRepository,
            IDbContext dbContext,
            IWebHelper webHelper)
        {
            this._languageRepository = languageRepository;
            this._userRepository = userRepository;
            this._storeRepository = storeRepository;
            this._dbContext = dbContext;
            this._webHelper = webHelper;
        }

        #endregion

        #region Utilities

        protected virtual void InstallLocaleResources()
        {
            //'English' language
            var language = _languageRepository.Table.Single(l => l.Name == "English");

            //save resources
            foreach (var filePath in System.IO.Directory.EnumerateFiles(CommonHelper.MapPath("~/App_Data/Localization/"), "*.Seagullres.xml", SearchOption.TopDirectoryOnly))
            {
                var localesXml = File.ReadAllText(filePath);
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                localizationService.ImportResourcesFromXml(language, localesXml);
            }

        }

        protected virtual void UpdateDefaultUser(string defaultUserEmail, string defaultUserPassword)
        {
            var adminUser = _userRepository.Table.Single(x => x.Email == "admin@yourStore.com");
            if (adminUser == null)
                throw new Exception("Admin user cannot be loaded");

            adminUser.UserGuid = Guid.NewGuid();
            adminUser.Email = defaultUserEmail;
            adminUser.Username = defaultUserEmail;
            _userRepository.Update(adminUser);

            var userRegistrationService = EngineContext.Current.Resolve<IUserRegistrationService>();
            userRegistrationService.ChangePassword(new ChangePasswordRequest(defaultUserEmail, false,
                 PasswordFormat.Hashed, defaultUserPassword));
        }

        protected virtual void UpdateDefaultStoreUrl()
        {
            var store = _storeRepository.Table.FirstOrDefault();
            if (store == null)
                throw new Exception("Default store cannot be loaded");

            store.Url = _webHelper.GetStoreLocation(false);
            _storeRepository.Update(store);
        }

        protected virtual void ExecuteSqlFile(string path)
        {
            var statements = new List<string>();

            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream))
            {
                string statement;
                while ((statement = ReadNextStatementFromStream(reader)) != null)
                    statements.Add(statement);
            }

            foreach (string stmt in statements)
                _dbContext.ExecuteSqlCommand(stmt);
        }

        protected virtual string ReadNextStatementFromStream(StreamReader reader)
        {
            var sb = new StringBuilder();
            while (true)
            {
                var lineOfText = reader.ReadLine();
                if (lineOfText == null)
                {
                    if (sb.Length > 0)
                        return sb.ToString();

                    return null;
                }

                if (lineOfText.TrimEnd().ToUpper() == "GO")
                    break;

                sb.Append(lineOfText + Environment.NewLine);
            }

            return sb.ToString();
        }

        #endregion

        #region Methods

        public virtual void InstallData(string defaultUserEmail,
            string defaultUserPassword, bool installSampleData = true)
        {
            ExecuteSqlFile(CommonHelper.MapPath("~/App_Data/Install/Fast/create_required_data.sql"));
            InstallLocaleResources();
            UpdateDefaultUser(defaultUserEmail, defaultUserPassword);
            UpdateDefaultStoreUrl();

            if (installSampleData)
            {
                ExecuteSqlFile(CommonHelper.MapPath("~/App_Data/Install/Fast/create_sample_data.sql"));
            }
        }

        #endregion
    }
}