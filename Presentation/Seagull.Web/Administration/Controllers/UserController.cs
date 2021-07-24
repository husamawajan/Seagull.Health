using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Seagull.Admin.Extensions;
using Seagull.Admin.Helpers;
using Seagull.Admin.Models.Common;
using Seagull.Admin.Models.Users;
using Seagull.Core;
using Seagull.Core.Caching;
using Seagull.Core.Domain.Common;
using Seagull.Core.Domain.Users;
using Seagull.Core.Domain.Directory;
using Seagull.Core.Domain.Messages;
using Seagull.Services;
using Seagull.Services.Authentication.External;
using Seagull.Services.Common;
using Seagull.Services.Users;
using Seagull.Services.Directory;
using Seagull.Services.ExportImport;
using Seagull.Services.Helpers;
using Seagull.Services.Localization;
using Seagull.Services.Logging;
using Seagull.Services.Messages;
using Seagull.Services.Security;
using Seagull.Services.Stores;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Controllers;
using Seagull.Web.Framework.Kendoui;
using Seagull.Web.Framework.Mvc;
using Seagull.Services.UserTypes;
using Seagull.Services.UserEntitys;

namespace Seagull.Admin.Controllers
{
    public partial class UserController : BaseAdminController
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IUserReportService _userReportService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressService _addressService;
        private readonly UserSettings _userSettings;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IExportManager _exportManager;
        private readonly IUserActivityService _userActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly AddressSettings _addressSettings;
        private readonly IStoreService _storeService;
        private readonly IUserAttributeParser _userAttributeParser;
        private readonly IUserAttributeService _userAttributeService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ICacheManager _cacheManager;
        private readonly IUserTypeService _userTypeService;
        private readonly IUserEntityService _userEntityService;
        #endregion

        #region Constructors

        public UserController(IUserService userService,
            IGenericAttributeService genericAttributeService,
            IUserRegistrationService userRegistrationService,
            IUserReportService userReportService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            DateTimeSettings dateTimeSettings,
            RewardPointsSettings rewardPointsSettings,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IAddressService addressService,
            UserSettings userSettings,
            IWorkContext workContext,
            IStoreContext storeContext,
            IExportManager exportManager,
            IUserActivityService userActivityService,
            IPermissionService permissionService,
            IQueuedEmailService queuedEmailService,
            EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService,

            IOpenAuthenticationService openAuthenticationService,
            AddressSettings addressSettings,
            IStoreService storeService,
            IUserAttributeParser userAttributeParser,
            IUserAttributeService userAttributeService,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            IAddressAttributeFormatter addressAttributeFormatter,
            IWorkflowMessageService workflowMessageService,
            ICacheManager cacheManager,
            IUserTypeService userTypeService,
            IUserEntityService userEntityService)
        {
            this._userService = userService;
            this._genericAttributeService = genericAttributeService;
            this._userRegistrationService = userRegistrationService;
            this._userReportService = userReportService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._dateTimeSettings = dateTimeSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._addressService = addressService;
            this._userSettings = userSettings;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._exportManager = exportManager;
            this._userActivityService = userActivityService;
            this._permissionService = permissionService;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountSettings = emailAccountSettings;
            this._emailAccountService = emailAccountService;
            this._openAuthenticationService = openAuthenticationService;
            this._addressSettings = addressSettings;
            this._storeService = storeService;
            this._userAttributeParser = userAttributeParser;
            this._userAttributeService = userAttributeService;
            this._addressAttributeParser = addressAttributeParser;
            this._addressAttributeService = addressAttributeService;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._workflowMessageService = workflowMessageService;
            this._cacheManager = cacheManager;
            this._userTypeService = userTypeService;
            this._userEntityService = userEntityService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual string GetUserRolesNames(IList<UserRole> userRoles, string separator = ",")
        {
            var sb = new StringBuilder();
            for (int i = 0; i < userRoles.Count; i++)
            {
                sb.Append(userRoles[i].Name);
                if (i != userRoles.Count - 1)
                {
                    sb.Append(separator);
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        [NonAction]
        protected virtual IList<RegisteredUserReportLineModel> GetReportRegisteredUsersModel()
        {
            var report = new List<RegisteredUserReportLineModel>();
            report.Add(new RegisteredUserReportLineModel
            {
                Period = _localizationService.GetResource("Admin.Users.Reports.RegisteredUsers.Fields.Period.7days"),
                Users = _userReportService.GetRegisteredUsersReport(7)
            });

            report.Add(new RegisteredUserReportLineModel
            {
                Period = _localizationService.GetResource("Admin.Users.Reports.RegisteredUsers.Fields.Period.14days"),
                Users = _userReportService.GetRegisteredUsersReport(14)
            });
            report.Add(new RegisteredUserReportLineModel
            {
                Period = _localizationService.GetResource("Admin.Users.Reports.RegisteredUsers.Fields.Period.month"),
                Users = _userReportService.GetRegisteredUsersReport(30)
            });
            report.Add(new RegisteredUserReportLineModel
            {
                Period = _localizationService.GetResource("Admin.Users.Reports.RegisteredUsers.Fields.Period.year"),
                Users = _userReportService.GetRegisteredUsersReport(365)
            });

            return report;
        }

        [NonAction]
        protected virtual IList<UserModel.AssociatedExternalAuthModel> GetAssociatedExternalAuthRecords(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var result = new List<UserModel.AssociatedExternalAuthModel>();
            foreach (var record in _openAuthenticationService.GetExternalIdentifiersFor(user))
            {
                var method = _openAuthenticationService.LoadExternalAuthenticationMethodBySystemName(record.ProviderSystemName);
                if (method == null)
                    continue;

                result.Add(new UserModel.AssociatedExternalAuthModel
                {
                    Id = record.Id,
                    Email = record.Email,
                    ExternalIdentifier = record.ExternalIdentifier,
                    AuthMethodName = method.PluginDescriptor.FriendlyName
                });
            }

            return result;
        }

        [NonAction]
        protected virtual UserModel PrepareUserModelForList(User user)
        {
            return new UserModel
            {
                Id = user.Id,
                Email = user.IsRegistered() ? user.Email : _localizationService.GetResource("Admin.Users.Guest"),
                Username = user.Username,
                FullName = user.GetFullName(),
                Phone = user.GetAttribute<string>(SystemUserAttributeNames.Phone),
                ZipPostalCode = user.GetAttribute<string>(SystemUserAttributeNames.ZipPostalCode),
                UserRoleNames = GetUserRolesNames(user.UserRoles.ToList()),
                Active = user.Active,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc),
                LastActivityDate = _dateTimeHelper.ConvertToUserTime(user.LastActivityDateUtc, DateTimeKind.Utc),
                EncId = string.Format("seagull={0}", UrlHelperExtensions.ActionEncodedCustom(new { id = user.Id }))
            };
        }

        [NonAction]
        protected virtual string ValidateUserRoles(IList<UserRole> userRoles)
        {
            if (userRoles == null)
                throw new ArgumentNullException("userRoles");

            //ensure a user is not added to both 'Guests' and 'Registered' user roles
            //ensure that a user is in at least one required role ('Guests' and 'Registered')
            //bool isInGuestsRole = userRoles.FirstOrDefault(cr => cr.SystemName == SystemUserRoleNames.Guests) != null;
            bool isInRegisteredRole = userRoles.FirstOrDefault(cr => cr.SystemName == SystemUserRoleNames.Registered) != null;
            //if (isInGuestsRole && isInRegisteredRole)
                //return _localizationService.GetResource("Admin.Users.Users.GuestsAndRegisteredRolesError");
            if (!isInRegisteredRole)//!isInGuestsRole && !isInRegisteredRole)
                return _localizationService.GetResource("Admin.Users.Users.AddUserToRegisteredRoleError"); // _localizationService.GetResource("Admin.Users.Users.AddUserToGuestsOrRegisteredRoleError");

            //no errors
            return "";
        }

        

        [NonAction]
        protected virtual void PrepareUserAttributeModel(UserModel model, User user)
        {
            var userAttributes = _userAttributeService.GetAllUserAttributes();
            foreach (var attribute in userAttributes)
            {
                var attributeModel = new UserModel.UserAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    IsRequired = attribute.IsRequired,
                   
                };


                //set already selected attributes
                if (user != null)
                {
                    var selectedUserAttributes = user.GetAttribute<string>(SystemUserAttributeNames.CustomUserAttributes, _genericAttributeService);
                }

                model.UserAttributes.Add(attributeModel);
            }
        }

        [NonAction]
        protected virtual string ParseCustomUserAttributes(FormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            string attributesXml = "";
            var userAttributes = _userAttributeService.GetAllUserAttributes();
            foreach (var attribute in userAttributes)
            {
                string controlId = string.Format("user_attribute_{0}", attribute.Id);
            }

            return attributesXml;
        }

        
        [NonAction]
        protected virtual void PrepareUserModel(UserModel model, User user, bool excludeProperties)
        {
            bool IsRtl = _workContext.WorkingLanguage.Rtl;

            var allStores = _storeService.GetAllStores();
            if (user != null)
            {
                model.Id = user.Id;
                if (!excludeProperties)
                {
                    model.Email = user.Email;
                    model.Username = user.Username;
                    model.AdminComment = user.AdminComment;
                    model.Active = user.Active;
                    model.UserOperatorId = user.UserOperatorId.Value;

                    if (user.RegisteredInStoreId == 0 || allStores.All(s => s.Id != user.RegisteredInStoreId))
                        model.RegisteredInStore = string.Empty;
                    else
                        model.RegisteredInStore = allStores.First(s => s.Id == user.RegisteredInStoreId).Name;

                    var affiliate = "";
                    if (affiliate != null)
                    {
                        model.AffiliateId = 0;
                        model.AffiliateName = "";
                    }

                    model.TimeZoneId = user.GetAttribute<string>(SystemUserAttributeNames.TimeZoneId);
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc);
                    model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(user.LastActivityDateUtc, DateTimeKind.Utc);
                    model.LastIpAddress = user.LastIpAddress;
                    model.LastVisitedPage = user.GetAttribute<string>(SystemUserAttributeNames.LastVisitedPage);

                    model.SelectedUserRoleIds = user.UserRoles.Select(cr => cr.Id).ToList();

                    

                    //form fields
                    model.FirstName = user.GetAttribute<string>(SystemUserAttributeNames.FirstName);
                    model.LastName = user.GetAttribute<string>(SystemUserAttributeNames.LastName);
                    model.Gender = user.GetAttribute<string>(SystemUserAttributeNames.Gender);
                    model.DateOfBirth = user.GetAttribute<DateTime?>(SystemUserAttributeNames.DateOfBirth);
                    model.StreetAddress = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress);
                    model.StreetAddress2 = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress2);
                    model.ZipPostalCode = user.GetAttribute<string>(SystemUserAttributeNames.ZipPostalCode);
                    model.City = user.GetAttribute<string>(SystemUserAttributeNames.City);
                    model.CountryId = user.GetAttribute<int>(SystemUserAttributeNames.CountryId);
                    model.StateProvinceId = user.GetAttribute<int>(SystemUserAttributeNames.StateProvinceId);
                    model.Phone = user.GetAttribute<string>(SystemUserAttributeNames.Phone);
                    model.Fax = user.GetAttribute<string>(SystemUserAttributeNames.Fax);
                }
            }

            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == model.TimeZoneId) });
            
            //user attributes
            PrepareUserAttributeModel(model, user);

            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.FaxEnabled = _userSettings.FaxEnabled;
            model.UserOperatorId = _workContext.CurrentUser.UserOperatorId.Value;
            model.UserMailId = _workContext.CurrentUser.UserMailId.Value;

            //countries and states
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries(showHidden: true))
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_userSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId).ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        bool anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = _localizationService.GetResource(anyCountrySelected ? "Admin.Address.OtherNonUS" : "Admin.Address.SelectState"),
                            Value = "0"
                        });
                    }
                }
            }

           
            //user roles
            var allRoles = _userService.GetAllUserRoles(true).Where(a => a.SystemName != SystemUserRoleNames.Registered);
            //if(_workContext.CurrentUser.UserRoles.Where(a => a.Name.Contains("Post department official")).Count() > 0)
            //{
            //    allRoles = allRoles.Where(a => a.Id == 70 || a.Id == 71);
            //}

            //var adminRole = allRoles.FirstOrDefault(c => c.SystemName == SystemUserRoleNames.Registered);
            ////precheck Registered Role as a default role while creating a new user through admin
            //if (user == null && adminRole != null)
            //{
            //    model.SelectedUserRoleIds.Add(adminRole.Id);
            //}

            foreach (var role in allRoles)
            {
                model.AvailableUserRoles.Add(new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Id.ToString(),
                    Selected = model.SelectedUserRoleIds.Contains(role.Id)
                });
            }
            //User Type
            foreach (var userType in _userTypeService.GetAllUserTypes())
            {
                model.AvailableUserTypes.Add(new SelectListItem
                {
                    Text = userType.Type,
                    Value = userType.Id.ToString(),
                    Selected = user != null ? user.UserTypeId.HasValue ? user.UserTypeId.Value == userType.Id ? true : false : false : false
                });
            }
            model.AvailableUserTypes.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Common.Select"),
                Value = "0",
                Selected = user != null ? !user.UserTypeId.HasValue || user.UserTypeId == 0 ? true : false : false,
            });
            model.AvailableUserTypes = model.AvailableUserTypes.OrderBy(a => a.Value).ToList();
            //User Entity
            if (user != null && user.UserTypeId.HasValue && user.UserTypeId > 0)
            {
                foreach (var userEntity in _userEntityService.GetAllUserEntitysByUserTypeId(user.UserTypeId.Value))
                {
                    model.AvailableEntityUsers.Add(new SelectListItem
                    {
                        Text = userEntity.Name,
                        Value = userEntity.Id.ToString(),
                        Selected = user.EntityUserId == userEntity.Id ? true : false
                    });
                }
                model.AvailableEntityUsers = model.AvailableEntityUsers.OrderBy(a => a.Value).ToList();
            }
            
            //external authentication records
            if (user != null)
            {
                model.AssociatedExternalAuthRecords = GetAssociatedExternalAuthRecords(user);
            }
            //sending of the welcome message:
            //1. "admin approval" registration method
            //2. already created user
            //3. registered
            model.AllowSendingOfWelcomeMessage = _userSettings.UserRegistrationType == UserRegistrationType.AdminApproval &&
                user != null &&
                user.IsRegistered();
            //sending of the activation message
            //1. "email validation" registration method
            //2. already created user
            //3. registered
            //4. not active
            model.AllowReSendingOfActivationMessage = _userSettings.UserRegistrationType == UserRegistrationType.EmailValidation &&
                user != null &&
                user.IsRegistered() &&
                !user.Active;
        }
        [NonAction]
        protected virtual void PrepareAddressModel(UserAddressModel model, Address address, User user, bool excludeProperties)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            model.UserId = user.Id;
            if (address != null)
            {
                if (!excludeProperties)
                {
                    model.Address = address.ToModel();
                }
            }

            if (model.Address == null)
                model.Address = new AddressModel();

            model.Address.FirstNameEnabled = true;
            model.Address.FirstNameRequired = true;
            model.Address.LastNameEnabled = true;
            model.Address.LastNameRequired = true;
            model.Address.EmailEnabled = true;
            model.Address.EmailRequired = true;
            model.Address.CountryEnabled = _addressSettings.CountryEnabled;
            model.Address.CountryRequired = _addressSettings.CountryEnabled; //country is required when enabled
            model.Address.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.Address.CityEnabled = _addressSettings.CityEnabled;
            model.Address.CityRequired = _addressSettings.CityRequired;
            model.Address.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.Address.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.Address.PhoneRequired = _addressSettings.PhoneRequired;
            model.Address.FaxEnabled = _addressSettings.FaxEnabled;
            model.Address.FaxRequired = _addressSettings.FaxRequired;
            //countries
            model.Address.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.Address.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.Address.CountryId) });
            //states
            var states = model.Address.CountryId.HasValue ? _stateProvinceService.GetStateProvincesByCountryId(model.Address.CountryId.Value, showHidden: true).ToList() : new List<StateProvince>();
            if (states.Any())
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.Address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
            //user attribute services
            model.Address.PrepareCustomAddressAttributes(address, _addressAttributeService, _addressAttributeParser);
        }

        [NonAction]
        private bool SecondAdminAccountExists(User user)
        {
            var users = _userService.GetAllUsers(userRoleIds: new[] { _userService.GetUserRoleBySystemName(SystemUserRoleNames.Administrators).Id });

            return users.Any(c => c.Active && c.Id != user.Id);
        }
        #endregion

        #region Users

        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //load registered users by default
            //var defaultRoleIds = new List<int> { _userService.GetUserRoleBySystemName(SystemUserRoleNames.Registered).Id };
            var model = new UserListModel
            {
                UsernamesEnabled = _userSettings.UsernamesEnabled,
                DateOfBirthEnabled = _userSettings.DateOfBirthEnabled,
                PhoneEnabled = _userSettings.PhoneEnabled,
                ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled,
                SearchUserRoleIds = new List<int>()//defaultRoleIds,
            };
            var allRoles = _userService.GetAllUserRoles(true).Where(a => a.SystemName != SystemUserRoleNames.Registered);
            foreach (var role in allRoles)
            {
                model.AvailableUserRoles.Add(new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Id.ToString(),
                    Selected = false //defaultRoleIds.Any(x => x == role.Id)
                });
            }

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult UserList(DataSourceRequest command, UserListModel model,
            [ModelBinder(typeof(CommaSeparatedModelBinder))] int[] searchUserRoleIds)
        {

            bool isMailAdmin = _workContext.CurrentUser.UserRoles.Where(a => a.Name.Contains("Post department official")).Count() > 0;

            //we use own own binder for searchUserRoleIds property 
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedKendoGridJson();

            var searchDayOfBirth = 0;
            int searchMonthOfBirth = 0;
            if (!String.IsNullOrWhiteSpace(model.SearchDayOfBirth))
                searchDayOfBirth = Convert.ToInt32(model.SearchDayOfBirth);
              if (!String.IsNullOrWhiteSpace(model.SearchMonthOfBirth))
                searchMonthOfBirth = Convert.ToInt32(model.SearchMonthOfBirth);

            var users = _userService.GetAllUsers(
                userRoleIds: searchUserRoleIds,
                email: model.SearchEmail,
                username: model.SearchUsername,
                firstName: model.SearchFirstName,
                lastName: model.SearchLastName,
                dayOfBirth: searchDayOfBirth,
                monthOfBirth: searchMonthOfBirth,
                phone: model.SearchPhone,
                zipPostalCode: model.SearchZipPostalCode,
                ipAddress: model.SearchIpAddress,
                loadOnlyWithShoppingCart: false,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);



            var filterUsers = new PagedList<User>(users.Where(a => isMailAdmin ? a.UserOperatorId == -1 : a.UserOperatorId != -1).ToList(), users.PageIndex, 1000);
            //var filterUsers = new PagedList<User>(users.Where(a => isMailAdmin ? a.UserOperatorId == -1 : a.UserOperatorId != -1).ToList(), users.PageIndex, int.MaxValue);


            var gridModel = new DataSourceResult
            {
                Data = filterUsers.Select(PrepareUserModelForList),
                Total = filterUsers.TotalCount
            };

            return Json(gridModel);
        }

        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var model = new UserModel();
            PrepareUserModel(model, null, false);
            //default value
            model.Active = true;
            model.EntityUserId = 0;
            model.UserTypeId = 0;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        [ValidateInput(false)]
        public virtual ActionResult Create(UserModel model, bool continueEditing, FormCollection form)
        {
            bool isMailAdmin = _workContext.CurrentUser.UserRoles.Where(a => a.Name.Contains("Post department official")).Count() > 0;

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            if (!String.IsNullOrWhiteSpace(model.Email))
            {
                var cust2 = _userService.GetUserByEmail(model.Email);
                if (cust2 != null)
                    ModelState.AddModelError("", "Email is already registered");
            }
            if (!String.IsNullOrWhiteSpace(model.Username) & _userSettings.UsernamesEnabled)
            {
                var cust2 = _userService.GetUserByUsername(model.Username);
                if (cust2 != null)
                    ModelState.AddModelError("", "Username is already registered");
            }

            //validate user roles
            var allUserRoles = _userService.GetAllUserRoles(true);
            var newUserRoles = new List<UserRole>();
            foreach (var userRole in allUserRoles)
                if (model.SelectedUserRoleIds.Contains(userRole.Id))
                    newUserRoles.Add(userRole);

            //Add Register Role To Current User
            newUserRoles.Add(allUserRoles.Where(a => a.SystemName == SystemUserRoleNames.Registered).FirstOrDefault());

            var userRolesError = ValidateUserRoles(newUserRoles);
            if (!String.IsNullOrEmpty(userRolesError))
            {
                ModelState.AddModelError("", userRolesError);
                ErrorNotification(userRolesError, false);
            }

            // Ensure that valid email address is entered if Registered role is checked to avoid registered users with empty email address
            if (newUserRoles.Any() && newUserRoles.FirstOrDefault(c => c.SystemName == SystemUserRoleNames.Registered) != null && !CommonHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError("", _localizationService.GetResource("Admin.Users.Users.ValidEmailRequiredRegisteredRole"));
                ErrorNotification(_localizationService.GetResource("Admin.Users.Users.ValidEmailRequiredRegisteredRole"), false);
            }

            //custom user attributes
            var userAttributesXml = ParseCustomUserAttributes(form);
            if (newUserRoles.Any() && newUserRoles.FirstOrDefault(c => c.SystemName == SystemUserRoleNames.Registered) != null)
            {
                var userAttributeWarnings = _userAttributeParser.GetAttributeWarnings(userAttributesXml);
                foreach (var error in userAttributeWarnings)
                {
                    ModelState.AddModelError("", error);
                }
            }

            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserGuid = Guid.NewGuid(),
                    Email = model.Email,
                    Username = model.Username,
                    AdminComment = model.AdminComment,
                    Active = model.Active,
                    CreatedOnUtc = DateTime.UtcNow,
                    LastActivityDateUtc = DateTime.UtcNow,
                    RegisteredInStoreId = _storeContext.CurrentStore.Id,
                    UserTypeId = model.UserTypeId,
                    EntityUserId = model.EntityUserId,
                    UserOperatorId = isMailAdmin ? -1
                                                 : model.UserOperatorId.HasValue ? model.UserOperatorId
                                                                                 : 0,
                    UserMailId = !isMailAdmin ? -1 
                                              : model.UserMailId.HasValue ? model.UserMailId.Value 
                                                                          : 0
                };
                _userService.InsertUser(user);

                //form fields
                if (_dateTimeSettings.AllowUsersToSetTimeZone)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.TimeZoneId, model.TimeZoneId);
                if (_userSettings.GenderEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Gender, model.Gender);
                _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.FirstName, model.FirstName);
                _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.LastName, model.LastName);
                if (_userSettings.DateOfBirthEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.DateOfBirth, model.DateOfBirth);
                if (_userSettings.StreetAddressEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StreetAddress, model.StreetAddress);
                if (_userSettings.StreetAddress2Enabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StreetAddress2, model.StreetAddress2);
                if (_userSettings.ZipPostalCodeEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.ZipPostalCode, model.ZipPostalCode);
                if (_userSettings.CityEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.City, model.City);
                if (_userSettings.CountryEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.CountryId, model.CountryId);
                if (_userSettings.CountryEnabled && _userSettings.StateProvinceEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StateProvinceId, model.StateProvinceId);
                if (_userSettings.PhoneEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Phone, model.Phone);
                if (_userSettings.FaxEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Fax, model.Fax);

                //custom user attributes
                _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.CustomUserAttributes, userAttributesXml);


                

                //password
                if (!String.IsNullOrWhiteSpace(model.Password))
                {
                    var changePassRequest = new ChangePasswordRequest(model.Email, false, _userSettings.DefaultPasswordFormat, model.Password);
                    var changePassResult = _userRegistrationService.ChangePassword(changePassRequest);
                    if (!changePassResult.Success)
                    {
                        foreach (var changePassError in changePassResult.Errors)
                            ErrorNotification(changePassError);
                    }
                }

                //user roles
                foreach (var userRole in newUserRoles)
                {
                    //ensure that the current user cannot add to "Administrators" system role if he's not an admin himself
                    if (userRole.SystemName == SystemUserRoleNames.Administrators &&
                        !_workContext.CurrentUser.IsAdmin())
                        continue;

                    user.UserRoles.Add(userRole);
                }

                //Add Register Role To Current User
                user.UserRoles.Add(allUserRoles.Where(a => a.SystemName == SystemUserRoleNames.Registered).FirstOrDefault());

                _userService.UpdateUser(user);
                //activity log
                _userActivityService.InsertActivity("AddNewUser", _localizationService.GetResource("ActivityLog.AddNewUser"), user.Id);

                SuccessNotification(_localizationService.GetResource("Admin.Users.Users.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { seagull = UrlHelperExtensions.ActionEncodedCustom(new { id = user.Id }) });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareUserModel(model, null, true);
            return View(model);
        }

        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(id);
            if (user == null || user.Deleted)
                //No user found with the specified id
                return RedirectToAction("List");

            var model = new UserModel();
            PrepareUserModel(model, user, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        [ValidateInput(false)]
        public virtual ActionResult Edit(UserModel model, bool continueEditing, FormCollection form)
        {
            bool isMailAdmin = _workContext.CurrentUser.UserRoles.Where(a => a.Name.Contains("Post department official")).Count() > 0;

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null || user.Deleted)
                //No user found with the specified id
                return RedirectToAction("List");

            //validate user roles
            var allUserRoles = _userService.GetAllUserRoles(true);
            var newUserRoles = new List<UserRole>();
            foreach (var userRole in allUserRoles)
                if (model.SelectedUserRoleIds.Contains(userRole.Id))
                    newUserRoles.Add(userRole);

            //Add Register Role To Current User
            newUserRoles.Add(allUserRoles.Where(a => a.SystemName == SystemUserRoleNames.Registered).FirstOrDefault());

            var userRolesError = ValidateUserRoles(newUserRoles);
            if (!String.IsNullOrEmpty(userRolesError))
            {
                ModelState.AddModelError("", userRolesError);
                ErrorNotification(userRolesError, false);
            }

            // Ensure that valid email address is entered if Registered role is checked to avoid registered users with empty email address
            if (newUserRoles.Any() && newUserRoles.FirstOrDefault(c => c.SystemName == SystemUserRoleNames.Registered) != null && !CommonHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError("", _localizationService.GetResource("Admin.Users.Users.ValidEmailRequiredRegisteredRole"));
                ErrorNotification(_localizationService.GetResource("Admin.Users.Users.ValidEmailRequiredRegisteredRole"), false);
            }

            //custom user attributes
            var userAttributesXml = ParseCustomUserAttributes(form);
            if (newUserRoles.Any() && newUserRoles.FirstOrDefault(c => c.SystemName == SystemUserRoleNames.Registered) != null)
            {
                var userAttributeWarnings = _userAttributeParser.GetAttributeWarnings(userAttributesXml);
                foreach (var error in userAttributeWarnings)
                {
                    ModelState.AddModelError("", error);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.AdminComment = model.AdminComment;

                    //prevent deactivation of the last active administrator
                    if (!user.IsAdmin() || model.Active || SecondAdminAccountExists(user))
                        user.Active = model.Active;
                    else
                        ErrorNotification(_localizationService.GetResource("Admin.Users.Users.AdminAccountShouldExists.Deactivate"));

                    //email
                    if (!String.IsNullOrWhiteSpace(model.Email))
                    {
                        _userRegistrationService.SetEmail(user, model.Email, false);
                    }
                    else
                    {
                        user.Email = model.Email;
                    }

                    //username
                    if (_userSettings.UsernamesEnabled)
                    {
                        if (!String.IsNullOrWhiteSpace(model.Username))
                        {
                            _userRegistrationService.SetUsername(user, model.Username);
                        }
                        else
                        {
                            user.Username = model.Username;
                        }
                    }

                    
                    //form fields
                    if (_dateTimeSettings.AllowUsersToSetTimeZone)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.TimeZoneId, model.TimeZoneId);
                    if (_userSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Gender, model.Gender);
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.FirstName, model.FirstName);
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.LastName, model.LastName);
                    if (_userSettings.DateOfBirthEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.DateOfBirth, model.DateOfBirth);
                    if (_userSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StreetAddress, model.StreetAddress);
                    if (_userSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StreetAddress2, model.StreetAddress2);
                    if (_userSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.ZipPostalCode, model.ZipPostalCode);
                    if (_userSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.City, model.City);
                    if (_userSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.CountryId, model.CountryId);
                    if (_userSettings.CountryEnabled && _userSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StateProvinceId, model.StateProvinceId);
                    if (_userSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Phone, model.Phone);
                    if (_userSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Fax, model.Fax);

                    //custom user attributes
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.CustomUserAttributes, userAttributesXml);

                    user.UserTypeId = model.UserTypeId;
                    user.EntityUserId = model.EntityUserId;
                    //user roles
                    foreach (var userRole in allUserRoles)
                    {
                        //ensure that the current user cannot add/remove to/from "Administrators" system role
                        //if he's not an admin himself
                        if (userRole.SystemName == SystemUserRoleNames.Administrators &&
                            !_workContext.CurrentUser.IsAdmin())
                            continue;

                        if (model.SelectedUserRoleIds.Contains(userRole.Id))
                        {
                            //new role
                            if (user.UserRoles.Count(cr => cr.Id == userRole.Id) == 0)
                                user.UserRoles.Add(userRole);
                        }
                        else
                        {
                            //prevent attempts to delete the administrator role from the user, if the user is the last active administrator
                            if (userRole.SystemName == SystemUserRoleNames.Administrators && !SecondAdminAccountExists(user))
                            {
                                ErrorNotification(_localizationService.GetResource("Admin.Users.Users.AdminAccountShouldExists.DeleteRole"));
                                continue;
                            }

                            //remove role
                            if (user.UserRoles.Count(cr => cr.Id == userRole.Id) > 0)
                                user.UserRoles.Remove(userRole);
                        }
                    }

                    //Add Register Role To Current User
                    user.UserRoles.Add(allUserRoles.Where(a => a.SystemName == SystemUserRoleNames.Registered).FirstOrDefault());
                    user.UserOperatorId = isMailAdmin ? user.UserOperatorId.Value 
                                                      : model.UserOperatorId.HasValue ? model.UserOperatorId.Value 
                                                                                      : 0;
                    user.UserMailId = !isMailAdmin ? user.UserMailId.Value 
                                                   : model.UserMailId.HasValue ? model.UserMailId.Value 
                                                                               : 0;
                    _userService.UpdateUser(user);

                    //activity log
                    _userActivityService.InsertActivity("EditUser", _localizationService.GetResource("ActivityLog.EditUser"), user.Id);

                    SuccessNotification(_localizationService.GetResource("Admin.Users.Users.Updated"));
                    if (continueEditing)
                    {
                        //selected tab
                        SaveSelectedTabName();

                        return RedirectToAction("Edit", new { seagull = UrlHelperExtensions.ActionEncodedCustom(new { id = user.Id }) });
                    }
                    return RedirectToAction("List");
                }
                catch (Exception exc)
                {
                    ErrorNotification(exc.Message, false);
                }
            }


            //If we got this far, something failed, redisplay form
            PrepareUserModel(model, user, true);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("changepassword")]
        public virtual ActionResult ChangePassword(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            //ensure that the current user cannot change passwords of "Administrators" if he's not an admin himself
            if (user.IsAdmin() && !_workContext.CurrentUser.IsAdmin())
            {
                ErrorNotification(_localizationService.GetResource("Admin.Users.Users.OnlyAdminCanChangePassword"));
                return RedirectToAction("Edit", new { id = user.Id });
            }

            if (ModelState.IsValid)
            {
                var changePassRequest = new ChangePasswordRequest(model.Email,
                    false, _userSettings.DefaultPasswordFormat, model.Password);
                var changePassResult = _userRegistrationService.ChangePassword(changePassRequest);
                if (changePassResult.Success)
                    SuccessNotification(_localizationService.GetResource("Admin.Users.Users.PasswordChanged"));
                else
                    foreach (var error in changePassResult.Errors)
                        ErrorNotification(error);
            }

            return RedirectToAction("Edit", new { id = user.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("remove-affiliate")]
        public virtual ActionResult RemoveAffiliate(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            _userService.UpdateUser(user);

            return RedirectToAction("Edit", new { id = user.Id });
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            try
            {
                //prevent attempts to delete the user, if it is the last active administrator
                if (user.IsAdmin() && !SecondAdminAccountExists(user))
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Users.Users.AdminAccountShouldExists.DeleteAdministrator"));
                    return RedirectToAction("Edit", new { id = user.Id });
                }

                //ensure that the current user cannot delete "Administrators" if he's not an admin himself
                if (user.IsAdmin() && !_workContext.CurrentUser.IsAdmin())
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Users.Users.OnlyAdminCanDeleteAdmin"));
                    return RedirectToAction("Edit", new { id = user.Id });
                }

                //delete
                _userService.DeleteUser(user);
                //activity log
                _userActivityService.InsertActivity("DeleteUser", _localizationService.GetResource("ActivityLog.DeleteUser"), user.Id);

                SuccessNotification(_localizationService.GetResource("Admin.Users.Users.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = user.Id });
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("impersonate")]
        public virtual ActionResult Impersonate(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AllowUserImpersonation))
                return AccessDeniedView();

            var user = _userService.GetUserById(id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            //ensure that a non-admin user cannot impersonate as an administrator
            //otherwise, that user can simply impersonate as an administrator and gain additional administrative privileges
            if (!_workContext.CurrentUser.IsAdmin() && user.IsAdmin())
            {
                ErrorNotification(_localizationService.GetResource("Admin.Users.Users.NonAdminNotImpersonateAsAdminError"));
                return RedirectToAction("Edit", user.Id);
            }

            //activity log
            _userActivityService.InsertActivity("Impersonation.Started", _localizationService.GetResource("ActivityLog.Impersonation.Started.StoreOwner"), user.Email, user.Id);
            _userActivityService.InsertActivity(user, "Impersonation.Started", _localizationService.GetResource("ActivityLog.Impersonation.Started.User"), _workContext.CurrentUser.Email, _workContext.CurrentUser.Id);

            //ensure login is not required
            user.RequireReLogin = false;
            _userService.UpdateUser(user);
            _genericAttributeService.SaveAttribute<int?>(_workContext.CurrentUser, SystemUserAttributeNames.ImpersonatedUserId, user.Id);

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("send-welcome-message")]
        public virtual ActionResult SendWelcomeMessage(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            _workflowMessageService.SendUserWelcomeMessage(user, _workContext.WorkingLanguage.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Users.Users.SendWelcomeMessage.Success"));

            return RedirectToAction("Edit", new { id = user.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("resend-activation-message")]
        public virtual ActionResult ReSendActivationMessage(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            //email validation message
            _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.AccountActivationToken, Guid.NewGuid().ToString());
            _workflowMessageService.SendUserEmailValidationMessage(user, _workContext.WorkingLanguage.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Users.Users.ReSendActivationMessage.Success"));

            return RedirectToAction("Edit", new { id = user.Id });
        }

        public virtual ActionResult SendEmail(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            try
            {
                if (String.IsNullOrWhiteSpace(user.Email))
                    throw new SeagullException("User email is empty");
                if (!CommonHelper.IsValidEmail(user.Email))
                    throw new SeagullException("User email is not valid");
                if (String.IsNullOrWhiteSpace(model.SendEmail.Subject))
                    throw new SeagullException("Email subject is empty");
                if (String.IsNullOrWhiteSpace(model.SendEmail.Body))
                    throw new SeagullException("Email body is empty");

                var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                if (emailAccount == null)
                    emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
                if (emailAccount == null)
                    throw new SeagullException("Email account can't be loaded");
                var email = new QueuedEmail
                {
                    Priority = QueuedEmailPriority.High,
                    EmailAccountId = emailAccount.Id,
                    FromName = emailAccount.DisplayName,
                    From = emailAccount.Email,
                    ToName = user.GetFullName(),
                    To = user.Email,
                    Subject = model.SendEmail.Subject,
                    Body = model.SendEmail.Body,
                    CreatedOnUtc = DateTime.UtcNow,
                    DontSendBeforeDateUtc = (model.SendEmail.SendImmediately || !model.SendEmail.DontSendBeforeDate.HasValue) ?
                        null : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SendEmail.DontSendBeforeDate.Value)
                };
                _queuedEmailService.InsertQueuedEmail(email);
                SuccessNotification(_localizationService.GetResource("Admin.Users.Users.SendEmail.Queued"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
            }

            return RedirectToAction("Edit", new { id = user.Id });
        }

        public virtual ActionResult SendPm(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            try
            {
                //if (user.IsGuest())
                //    throw new SeagullException("User should be registered");
                if (String.IsNullOrWhiteSpace(model.SendPm.Subject))
                    throw new SeagullException("PM subject is empty");
                if (String.IsNullOrWhiteSpace(model.SendPm.Message))
                    throw new SeagullException("PM message is empty");

                SuccessNotification(_localizationService.GetResource("Admin.Users.Users.SendPM.Sent"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
            }

            return RedirectToAction("Edit", new { id = user.Id });
        }

        #endregion


        #region Addresses

        [HttpPost]
        public virtual ActionResult AddressesSelect(int userId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedKendoGridJson();

            var user = _userService.GetUserById(userId);
            if (user == null)
                throw new ArgumentException("No user found with the specified id", "userId");

            var addresses = user.Addresses.OrderByDescending(a => a.CreatedOnUtc).ThenByDescending(a => a.Id).ToList();
            var gridModel = new DataSourceResult
            {
                Data = addresses.Select(x =>
                {
                    var model = x.ToModel();
                    var addressHtmlSb = new StringBuilder("<div>");
                   
                    if (_addressSettings.StreetAddressEnabled && !String.IsNullOrEmpty(model.Address1))
                        addressHtmlSb.AppendFormat("{0}<br />", Server.HtmlEncode(model.Address1));
                    if (_addressSettings.StreetAddress2Enabled && !String.IsNullOrEmpty(model.Address2))
                        addressHtmlSb.AppendFormat("{0}<br />", Server.HtmlEncode(model.Address2));
                    if (_addressSettings.CityEnabled && !String.IsNullOrEmpty(model.City))
                        addressHtmlSb.AppendFormat("{0},", Server.HtmlEncode(model.City));
                    if (_addressSettings.StateProvinceEnabled && !String.IsNullOrEmpty(model.StateProvinceName))
                        addressHtmlSb.AppendFormat("{0},", Server.HtmlEncode(model.StateProvinceName));
                    if (_addressSettings.ZipPostalCodeEnabled && !String.IsNullOrEmpty(model.ZipPostalCode))
                        addressHtmlSb.AppendFormat("{0}<br />", Server.HtmlEncode(model.ZipPostalCode));
                    if (_addressSettings.CountryEnabled && !String.IsNullOrEmpty(model.CountryName))
                        addressHtmlSb.AppendFormat("{0}", Server.HtmlEncode(model.CountryName));
                    var customAttributesFormatted = _addressAttributeFormatter.FormatAttributes(x.CustomAttributes);
                    if (!String.IsNullOrEmpty(customAttributesFormatted))
                    {
                        //already encoded
                        addressHtmlSb.AppendFormat("<br />{0}", customAttributesFormatted);
                    }
                    addressHtmlSb.Append("</div>");
                    model.AddressHtml = addressHtmlSb.ToString();
                    return model;
                }),
                Total = addresses.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult AddressDelete(int id, int userId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(userId);
            if (user == null)
                throw new ArgumentException("No user found with the specified id", "userId");

            var address = user.Addresses.FirstOrDefault(a => a.Id == id);
            if (address == null)
                //No user found with the specified id
                return Content("No user found with the specified id");
            user.RemoveAddress(address);
            _userService.UpdateUser(user);
            //now delete the address record
            _addressService.DeleteAddress(address);

            return new NullJsonResult();
        }

        public virtual ActionResult AddressCreate(int userId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(userId);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            var model = new UserAddressModel();
            PrepareAddressModel(model, null, user, false);

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult AddressCreate(UserAddressModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.UserId);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            //custom address attributes
            var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity();
                address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                user.Addresses.Add(address);
                _userService.UpdateUser(user);

                SuccessNotification(_localizationService.GetResource("Admin.Users.Users.Addresses.Added"));
                return RedirectToAction("AddressEdit", new { addressId = address.Id, userId = model.UserId });
            }

            //If we got this far, something failed, redisplay form
            PrepareAddressModel(model, null, user, true);
            return View(model);
        }

        public virtual ActionResult AddressEdit(int addressId, int userId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(userId);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            var address = _addressService.GetAddressById(addressId);
            if (address == null)
                //No address found with the specified id
                return RedirectToAction("Edit", new { id = user.Id });

            var model = new UserAddressModel();
            PrepareAddressModel(model, address, user, false);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult AddressEdit(UserAddressModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.UserId);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            var address = _addressService.GetAddressById(model.Address.Id);
            if (address == null)
                //No address found with the specified id
                return RedirectToAction("Edit", new { id = user.Id });

            //custom address attributes
            var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;
                _addressService.UpdateAddress(address);

                SuccessNotification(_localizationService.GetResource("Admin.Users.Users.Addresses.Updated"));
                return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, userId = model.UserId });
            }

            //If we got this far, something failed, redisplay form
            PrepareAddressModel(model, address, user, true);

            return View(model);
        }

        #endregion

        

        

        

        #region Activity log

        [HttpPost]
        public virtual ActionResult ListActivityLog(DataSourceRequest command, int userId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedKendoGridJson();

            var activityLog = _userActivityService.GetAllActivities(null, null, userId, 0, command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = activityLog.Select(x =>
                {
                    var m = new UserModel.ActivityLogModel
                    {
                        Id = x.Id,
                        ActivityLogTypeName = x.ActivityLogType.Name,
                        Comment = x.Comment,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc),
                        IpAddress = x.IpAddress
                    };
                    return m;

                }),
                Total = activityLog.TotalCount
            };

            return Json(gridModel);
        }

        #endregion

        

        #region Export / Import

        [HttpPost, ActionName("List")]
        [FormValueRequired("exportexcel-all")]
        public virtual ActionResult ExportExcelAll(UserListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var searchDayOfBirth = 0;
            int searchMonthOfBirth = 0;
            if (!String.IsNullOrWhiteSpace(model.SearchDayOfBirth))
                searchDayOfBirth = Convert.ToInt32(model.SearchDayOfBirth);
            if (!String.IsNullOrWhiteSpace(model.SearchMonthOfBirth))
                searchMonthOfBirth = Convert.ToInt32(model.SearchMonthOfBirth);

            var users = _userService.GetAllUsers(
                userRoleIds: model.SearchUserRoleIds.ToArray(),
                email: model.SearchEmail,
                username: model.SearchUsername,
                firstName: model.SearchFirstName,
                lastName: model.SearchLastName,
                dayOfBirth: searchDayOfBirth,
                monthOfBirth: searchMonthOfBirth,
                phone: model.SearchPhone,
                zipPostalCode: model.SearchZipPostalCode,
                loadOnlyWithShoppingCart: false);

            try
            {
                byte[] bytes = _exportManager.ExportUsersToXlsx(users);
                return File(bytes, MimeTypes.TextXlsx, "users.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual ActionResult ExportExcelSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var users = new List<User>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                users.AddRange(_userService.GetUsersByIds(ids));
            }

            try
            {
                byte[] bytes = _exportManager.ExportUsersToXlsx(users);
                return File(bytes, MimeTypes.TextXlsx, "users.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("exportxml-all")]
        public virtual ActionResult ExportXmlAll(UserListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var searchDayOfBirth = 0;
            int searchMonthOfBirth = 0;
            if (!String.IsNullOrWhiteSpace(model.SearchDayOfBirth))
                searchDayOfBirth = Convert.ToInt32(model.SearchDayOfBirth);
            if (!String.IsNullOrWhiteSpace(model.SearchMonthOfBirth))
                searchMonthOfBirth = Convert.ToInt32(model.SearchMonthOfBirth);

            var users = _userService.GetAllUsers(
                userRoleIds: model.SearchUserRoleIds.ToArray(),
                email: model.SearchEmail,
                username: model.SearchUsername,
                firstName: model.SearchFirstName,
                lastName: model.SearchLastName,
                dayOfBirth: searchDayOfBirth,
                monthOfBirth: searchMonthOfBirth,
                phone: model.SearchPhone,
                zipPostalCode: model.SearchZipPostalCode,
                loadOnlyWithShoppingCart: false);

            try
            {
                var xml = _exportManager.ExportUsersToXml(users);
                return new XmlDownloadResult(xml, "users.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual ActionResult ExportXmlSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var users = new List<User>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                users.AddRange(_userService.GetUsersByIds(ids));
            }

            var xml = _exportManager.ExportUsersToXml(users);
            return new XmlDownloadResult(xml, "users.xml");
        }

        #endregion
    }
}
