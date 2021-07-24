//Contributor:  Nicholas Mayne

using System;
using System.Collections.Generic;
using System.Linq;
using Seagull.Core.Data;
using Seagull.Core.Domain.Users;
using Seagull.Core.Plugins;
using Seagull.Services.Users;

namespace Seagull.Services.Authentication.External
{
    /// <summary>
    /// Open authentication service
    /// </summary>
    public partial class OpenAuthenticationService : IOpenAuthenticationService
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly IRepository<ExternalAuthenticationRecord> _externalAuthenticationRecordRepository;

        #endregion

        #region Ctor

        public OpenAuthenticationService(IRepository<ExternalAuthenticationRecord> externalAuthenticationRecordRepository,
            IPluginFinder pluginFinder,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            IUserService userService)
        {
            this._externalAuthenticationRecordRepository = externalAuthenticationRecordRepository;
            this._pluginFinder = pluginFinder;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
            this._userService = userService;
        }

        #endregion

        #region Methods

        #region External authentication methods

        /// <summary>
        /// Load active external authentication methods
        /// </summary>
        /// <param name="user">Load records allowed only to a specified user; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Payment methods</returns>
        public virtual IList<IExternalAuthenticationMethod> LoadActiveExternalAuthenticationMethods(User user = null, int storeId = 0)
        {
            return LoadAllExternalAuthenticationMethods(user, storeId)
                .Where(provider => _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames
                    .Contains(provider.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase)).ToList();
        }

        /// <summary>
        /// Load external authentication method by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found external authentication method</returns>
        public virtual IExternalAuthenticationMethod LoadExternalAuthenticationMethodBySystemName(string systemName)
        {
            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<IExternalAuthenticationMethod>(systemName);
            if (descriptor != null)
                return descriptor.Instance<IExternalAuthenticationMethod>();

            return null;
        }

        /// <summary>
        /// Load all external authentication methods
        /// </summary>
        /// <param name="user">Load records allowed only to a specified user; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>External authentication methods</returns>
        public virtual IList<IExternalAuthenticationMethod> LoadAllExternalAuthenticationMethods(User user = null, int storeId = 0)
        {
            return _pluginFinder.GetPlugins<IExternalAuthenticationMethod>(user: user, storeId: storeId).ToList();
        }

        #endregion

        /// <summary>
        /// Accociate external account with user
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="parameters">Open authentication parameters</param>
        public virtual void AssociateExternalAccountWithUser(User user, OpenAuthenticationParameters parameters)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            //find email
            string email = null;
            if (parameters.UserClaims != null)
                foreach (var userClaim in parameters.UserClaims
                    .Where(x => x.Contact != null && !String.IsNullOrEmpty(x.Contact.Email)))
                    {
                        //found
                        email = userClaim.Contact.Email;
                        break;
                    }

            var externalAuthenticationRecord = new ExternalAuthenticationRecord
            {
                UserId = user.Id,
                Email = email,
                ExternalIdentifier = parameters.ExternalIdentifier,
                ExternalDisplayIdentifier = parameters.ExternalDisplayIdentifier,
                OAuthToken = parameters.OAuthToken,
                OAuthAccessToken = parameters.OAuthAccessToken,
                ProviderSystemName = parameters.ProviderSystemName,
            };

            _externalAuthenticationRecordRepository.Insert(externalAuthenticationRecord);
        }

        /// <summary>
        /// Check that account exists
        /// </summary>
        /// <param name="parameters">Open authentication parameters</param>
        /// <returns>True if it exists; otherwise false</returns>
        public virtual bool AccountExists(OpenAuthenticationParameters parameters)
        {
            return GetUser(parameters) != null;
        }

        /// <summary>
        /// Get the particular user with specified parameters
        /// </summary>
        /// <param name="parameters">Open authentication parameters</param>
        /// <returns>User</returns>
        public virtual User GetUser(OpenAuthenticationParameters parameters)
        {
            var record = _externalAuthenticationRecordRepository.Table
                .FirstOrDefault(o => o.ExternalIdentifier == parameters.ExternalIdentifier && 
                    o.ProviderSystemName == parameters.ProviderSystemName);

            if (record != null)
                return _userService.GetUserById(record.UserId);

            return null;
        }

        /// <summary>
        /// Get external authentication records for the specified user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>List of external authentication records</returns>
        public virtual IList<ExternalAuthenticationRecord> GetExternalIdentifiersFor(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return user.ExternalAuthenticationRecords.ToList();
        }

        /// <summary>
        /// Delete the external authentication record
        /// </summary>
        /// <param name="externalAuthenticationRecord">External authentication record</param>
        public virtual void DeleteExternalAuthenticationRecord(ExternalAuthenticationRecord externalAuthenticationRecord)
        {
            if (externalAuthenticationRecord == null)
                throw new ArgumentNullException("externalAuthenticationRecord");

            _externalAuthenticationRecordRepository.Delete(externalAuthenticationRecord);
        }

        /// <summary>
        /// Remove the association
        /// </summary>
        /// <param name="parameters">Open authentication parameters</param>
        public virtual void RemoveAssociation(OpenAuthenticationParameters parameters)
        {
            var record = _externalAuthenticationRecordRepository.Table
                .FirstOrDefault(o => o.ExternalIdentifier == parameters.ExternalIdentifier &&
                    o.ProviderSystemName == parameters.ProviderSystemName);

            if (record != null)
                _externalAuthenticationRecordRepository.Delete(record);
        }

        #endregion
    }
}