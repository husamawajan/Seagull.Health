using System.Collections.Generic;
using Seagull.Core.Domain.Users;
using Seagull.Core.Domain.Security;

namespace Seagull.Services.Security
{
    /// <summary>
    /// Standard permission provider
    /// </summary>
    public partial class StandardPermissionProvider : IPermissionProvider
    {
        //admin area permissions
        public static readonly PermissionRecord AccessAdminPanel = new PermissionRecord { Name = "Access admin area", SystemName = "AccessAdminPanel", Category = "Standard" };
        public static readonly PermissionRecord AllowUserImpersonation = new PermissionRecord { Name = "Admin area. Allow User Impersonation", SystemName = "AllowUserImpersonation", Category = "Users" };
        public static readonly PermissionRecord ManagePolls = new PermissionRecord { Name = "Admin area. Manage Polls", SystemName = "ManagePolls", Category = "Content Management" };
        public static readonly PermissionRecord ManageNews = new PermissionRecord { Name = "Admin area. Manage News", SystemName = "ManageNews", Category = "Content Management" };
        public static readonly PermissionRecord ManageBlog = new PermissionRecord { Name = "Admin area. Manage Blog", SystemName = "ManageBlog", Category = "Content Management" };
        public static readonly PermissionRecord ManageWidgets = new PermissionRecord { Name = "Admin area. Manage Widgets", SystemName = "ManageWidgets", Category = "Content Management" };
        public static readonly PermissionRecord ManageTopics = new PermissionRecord { Name = "Admin area. Manage Topics", SystemName = "ManageTopics", Category = "Content Management" };
        public static readonly PermissionRecord ManageForums = new PermissionRecord { Name = "Admin area. Manage Forums", SystemName = "ManageForums", Category = "Content Management" };
        public static readonly PermissionRecord ManageMessageTemplates = new PermissionRecord { Name = "Admin area. Manage Message Templates", SystemName = "ManageMessageTemplates", Category = "Content Management" };
        public static readonly PermissionRecord ManageCountries = new PermissionRecord { Name = "Admin area. Manage Countries", SystemName = "ManageCountries", Category = "Configuration" };
        public static readonly PermissionRecord ManageLanguages = new PermissionRecord { Name = "Admin area. Manage Languages", SystemName = "ManageLanguages", Category = "Configuration" };
        public static readonly PermissionRecord ManageSettings = new PermissionRecord { Name = "Admin area. Manage Settings", SystemName = "ManageSettings", Category = "Configuration" };
        public static readonly PermissionRecord ManageExternalAuthenticationMethods = new PermissionRecord { Name = "Admin area. Manage External Authentication Methods", SystemName = "ManageExternalAuthenticationMethods", Category = "Configuration" };
        public static readonly PermissionRecord ManageCurrencies = new PermissionRecord { Name = "Admin area. Manage Currencies", SystemName = "ManageCurrencies", Category = "Configuration" };
        public static readonly PermissionRecord ManageEmailAccounts = new PermissionRecord { Name = "Admin area. Manage Email Accounts", SystemName = "ManageEmailAccounts", Category = "Configuration" };
        public static readonly PermissionRecord ManageStores = new PermissionRecord { Name = "Admin area. Manage Stores", SystemName = "ManageStores", Category = "Configuration" };
        public static readonly PermissionRecord ManagePlugins = new PermissionRecord { Name = "Admin area. Manage Plugins", SystemName = "ManagePlugins", Category = "Configuration" };
        public static readonly PermissionRecord ManageSystemLog = new PermissionRecord { Name = "Admin area. Manage System Log", SystemName = "ManageSystemLog", Category = "Configuration" };
        public static readonly PermissionRecord ManageMessageQueue = new PermissionRecord { Name = "Admin area. Manage Message Queue", SystemName = "ManageMessageQueue", Category = "Configuration" };
        public static readonly PermissionRecord ManageMaintenance = new PermissionRecord { Name = "Admin area. Manage Maintenance", SystemName = "ManageMaintenance", Category = "Configuration" };
        public static readonly PermissionRecord HtmlEditorManagePictures = new PermissionRecord { Name = "Admin area. HTML Editor. Manage pictures", SystemName = "HtmlEditor.ManagePictures", Category = "Configuration" };
        public static readonly PermissionRecord ManageScheduleTasks = new PermissionRecord { Name = "Admin area. Manage Schedule Tasks", SystemName = "ManageScheduleTasks", Category = "Configuration" };
        public static readonly PermissionRecord ManageOperator = new PermissionRecord { Name = "Admin.area.Operator", SystemName = "ManageOperator", Category = "Lookups" };
        public static readonly PermissionRecord ManageQTitle = new PermissionRecord { Name = "Admin.area.QTitle", SystemName = "ManageQTitle", Category = "Lookups" };
        public static readonly PermissionRecord ManageMainQuestions = new PermissionRecord { Name = "Admin.area.MainQuestions", SystemName = "ManageMainQuestions", Category = "Lookups" };
        public static readonly PermissionRecord ManageSubQuestions = new PermissionRecord { Name = "Admin.area.SubQuestions", SystemName = "ManageSubQuestions", Category = "Lookups" };
        public static readonly PermissionRecord ManageService = new PermissionRecord { Name = "Admin.area.Service", SystemName = "ManageService", Category = "Lookups" };
        public static readonly PermissionRecord ManageSubService = new PermissionRecord { Name = "Admin.area.SubService", SystemName = "ManageSubService", Category = "Lookups" };
        public static readonly PermissionRecord ManageSubsidiaryServices = new PermissionRecord { Name = "Admin.area.SubsidiaryServices", SystemName = "ManageSubsidiaryServices", Category = "Lookups" };
        public static readonly PermissionRecord ManageSurvey = new PermissionRecord { Name = "Admin.area.Survey", SystemName = "ManageSurvey", Category = "Lookups" };
        public static readonly PermissionRecord ManageQReport = new PermissionRecord { Name = "Admin.area.QReport", SystemName = "ManageQReport", Category = "Reports" };
        public static readonly PermissionRecord ManageSurveyReport = new PermissionRecord { Name = "Admin.area.SurveyReport", SystemName = "ManageSurveyReport", Category = "Reports" };
        public static readonly PermissionRecord ManageAnnualQTitle = new PermissionRecord { Name = "Admin.area.AnnualQTitle", SystemName = "ManageAnnualQTitle", Category = "Lookups" };
        public static readonly PermissionRecord ManageAnnualQReport = new PermissionRecord { Name = "Admin.area.AnnualQReport", SystemName = "ManageAnnualQReport", Category = "Reports" };
        public static readonly PermissionRecord ManageSystemReport = new PermissionRecord { Name = "Admin.area.SystemReport", SystemName = "ManageSystemReport", Category = "Lookups" };
        public static readonly PermissionRecord ManageAdminUserManual = new PermissionRecord { Name = "Admin area. ManageAdminUserManual", SystemName = "ManageAdminUserManual", Category = "UserManual" };
        public static readonly PermissionRecord ManageEntityUserManual = new PermissionRecord { Name = "Admin area.ManageEntityUserManual", SystemName = "ManageEntityUserManual", Category = "UserManual" };

        #region Public Permissions

        //public store permissions
        public static readonly PermissionRecord DisplayPrices = new PermissionRecord { Name = "Public store. Display Prices", SystemName = "DisplayPrices", Category = "PublicStore" };
        public static readonly PermissionRecord EnableShoppingCart = new PermissionRecord { Name = "Public store. Enable shopping cart", SystemName = "EnableShoppingCart", Category = "PublicStore" };
        public static readonly PermissionRecord EnableWishlist = new PermissionRecord { Name = "Public store. Enable wishlist", SystemName = "EnableWishlist", Category = "PublicStore" };
        public static readonly PermissionRecord PublicStoreAllowNavigation = new PermissionRecord { Name = "Public store. Allow navigation", SystemName = "PublicStoreAllowNavigation", Category = "PublicStore" };
        public static readonly PermissionRecord AccessClosedStore = new PermissionRecord { Name = "Public store. Access a closed store", SystemName = "AccessClosedStore", Category = "PublicStore" };

        #endregion

        #region User Management

        //Users And UserRole
        public static readonly PermissionRecord ManageUsers = new PermissionRecord { Name = "Admin area. Manage Users", SystemName = "ManageUsers", Category = "Users" };

        //Change Password 
        public static readonly PermissionRecord ViewChangePassword = new PermissionRecord { Name = "Admin.Area.ViewChangePassword", SystemName = "ViewChangePassword", Category = "ChangePassword" };
        public static readonly PermissionRecord EditChangePassword = new PermissionRecord { Name = "Admin.Area.EditChangePassword", SystemName = "EditChangePassword", Category = "ChangePassword" };

        //User Types
        public static readonly PermissionRecord ViewUserTypeInSiteMap = new PermissionRecord { Name = "Admin.Area.ViewUserTypeInSiteMap", SystemName = "ViewUserTypeInSiteMap", Category = "UserType" };
        public static readonly PermissionRecord ViewUserType = new PermissionRecord { Name = "Admin.Area.ViewUserType", SystemName = "ViewUserType", Category = "UserType" };
        public static readonly PermissionRecord AddUserType = new PermissionRecord { Name = "Admin.Area.AddUserType", SystemName = "AddUserType", Category = "UserType" };
        public static readonly PermissionRecord EditUserType = new PermissionRecord { Name = "Admin.Area.EditUserType", SystemName = "EditUserType", Category = "UserType" };
        public static readonly PermissionRecord DeleteUserType = new PermissionRecord { Name = "Admin.Area.DeleteUserType", SystemName = "DeleteUserType", Category = "UserType" };

        //Entity Users
        public static readonly PermissionRecord ViewUserEntityInSiteMap = new PermissionRecord { Name = "Admin.Area.ViewUserEntityInSiteMap", SystemName = "ViewUserEntityInSiteMap", Category = "UserEntity" };
        public static readonly PermissionRecord ViewUserEntity = new PermissionRecord { Name = "Admin.Area.ViewUserEntity", SystemName = "ViewUserEntity", Category = "UserEntity" };
        public static readonly PermissionRecord AddUserEntity = new PermissionRecord { Name = "Admin.Area.AddUserEntity", SystemName = "AddUserEntity", Category = "UserEntity" };
        public static readonly PermissionRecord EditUserEntity = new PermissionRecord { Name = "Admin.Area.EditUserEntity", SystemName = "EditUserEntity", Category = "UserEntity" };
        public static readonly PermissionRecord DeleteUserEntity = new PermissionRecord { Name = "Admin.Area.DeleteUserEntity", SystemName = "DeleteUserEntity", Category = "UserEntity" };

        //ActivityLog
        public static readonly PermissionRecord ManageActivityLog = new PermissionRecord { Name = "Admin area. Manage Activity Log", SystemName = "ManageActivityLog", Category = "Configuration" };

        //Manage Acl-Permissions
        public static readonly PermissionRecord ManageAcl = new PermissionRecord { Name = "Admin area. Manage ACL", SystemName = "ManageACL", Category = "Configuration" };

        #endregion

        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                AccessAdminPanel,
                AllowUserImpersonation,
                ManageUsers,
                ManageNews,
                ManageBlog,
                ManageWidgets,
                ManageTopics,
                ManageForums,
                ManageMessageTemplates,
                ManageCountries,
                ManageLanguages,
                ManageSettings,
                ManageExternalAuthenticationMethods,
                ManageCurrencies,
                ManageActivityLog,
                ManageAcl,
                ManageEmailAccounts,
                ManageStores,
                ManagePlugins,
                ManageSystemLog,
                ManageMessageQueue,
                ManageMaintenance,
                HtmlEditorManagePictures,
                ManageScheduleTasks,
                DisplayPrices,
                EnableShoppingCart,
                EnableWishlist,
                PublicStoreAllowNavigation,
                AccessClosedStore
            };
        }

        public virtual IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissionRecord
                {
                    UserRoleSystemName = SystemUserRoleNames.Administrators,
                    PermissionRecords = new[]
                    {
                        AccessAdminPanel,
                        AllowUserImpersonation,
                        ManageUsers,
                        ManagePolls,
                        ManageNews,
                        ManageBlog,
                        ManageWidgets,
                        ManageTopics,
                        ManageForums,
                        ManageMessageTemplates,
                        ManageCountries,
                        ManageLanguages,
                        ManageSettings,
                        ManageExternalAuthenticationMethods,
                        ManageCurrencies,
                        ManageActivityLog,
                        ManageAcl,
                        ManageEmailAccounts,
                        ManageStores,
                        ManagePlugins,
                        ManageSystemLog,
                        ManageMessageQueue,
                        ManageMaintenance,
                        HtmlEditorManagePictures,
                        ManageScheduleTasks,
                        DisplayPrices,
                        EnableShoppingCart,
                        EnableWishlist,
                        PublicStoreAllowNavigation,
                        AccessClosedStore
                    }
                },
                //new DefaultPermissionRecord 
                //{
                //    UserRoleSystemName = SystemUserRoleNames.ForumModerators,
                //    PermissionRecords = new[] 
                //    {
                //        DisplayPrices,
                //        EnableShoppingCart,
                //        EnableWishlist,
                //        PublicStoreAllowNavigation
                //    }
                //},
                new DefaultPermissionRecord
                {
                    UserRoleSystemName = SystemUserRoleNames.Registered,
                    PermissionRecords = new[]
                    {
                        DisplayPrices,
                        EnableShoppingCart,
                        EnableWishlist,
                        PublicStoreAllowNavigation
                    }
                },

            };
        }
    }
}