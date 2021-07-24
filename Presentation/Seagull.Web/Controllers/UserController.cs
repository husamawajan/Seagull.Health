using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Seagull.Core;
using Seagull.Core.Domain;
using Seagull.Core.Domain.Common;
using Seagull.Core.Domain.Users;
using Seagull.Core.Domain.Localization;
using Seagull.Core.Domain.Media;
using Seagull.Core.Domain.Messages;
using Seagull.Services.Authentication;
using Seagull.Services.Authentication.External;
using Seagull.Services.Common;
using Seagull.Services.Users;
using Seagull.Services.Directory;
using Seagull.Services.Events;
using Seagull.Services.Helpers;
using Seagull.Services.Localization;
using Seagull.Services.Logging;
using Seagull.Services.Media;
using Seagull.Services.Messages;
using Seagull.Services.Stores;
using Seagull.Web.Extensions;
using Seagull.Web.Factories;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Controllers;
using Seagull.Web.Framework.Security;
using Seagull.Web.Framework.Security.Captcha;
using Seagull.Web.Framework.Security.Honeypot;
using Seagull.Web.Models.User;
using System.Threading;
using System.Net.Mail;
using System.Net;
using Seagull.Core.Domain.Emails;
using Seagull.Core.Infrastructure;
using Seagull.Services.Emails;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace Seagull.Web.Controllers
{
    public partial class UserController : BasePublicController
    {
        #region Fields

        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IUserModelFactory _userModelFactory;
        private readonly IAuthenticationService _authenticationService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IUserService _userService;
        private readonly IUserAttributeParser _userAttributeParser;
        private readonly IUserAttributeService _userAttributeService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly UserSettings _userSettings;
        private readonly AddressSettings _addressSettings;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IPictureService _pictureService;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly IWebHelper _webHelper;
        private readonly IUserActivityService _userActivityService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IStoreService _storeService;
        private readonly IEventPublisher _eventPublisher;
        private readonly MediaSettings _mediaSettings;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly IEmailService _emailservice;

        #endregion

        #region Ctor

        public UserController(IAddressModelFactory addressModelFactory,
            IUserModelFactory userModelFactory,
            IAuthenticationService authenticationService,
            DateTimeSettings dateTimeSettings,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IUserService userService,
            IUserAttributeParser userAttributeParser,
            IUserAttributeService userAttributeService,
            IGenericAttributeService genericAttributeService,
            IUserRegistrationService userRegistrationService,
            UserSettings userSettings,
            AddressSettings addressSettings,
            IAddressService addressService,
            ICountryService countryService,
            IPictureService pictureService,
            IOpenAuthenticationService openAuthenticationService,
            IWebHelper webHelper,
            IUserActivityService userActivityService,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            IStoreService storeService,
            IEventPublisher eventPublisher,
            MediaSettings mediaSettings,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            CaptchaSettings captchaSettings,
            StoreInformationSettings storeInformationSettings,
            IEmailService emailservice)
        {
            this._addressModelFactory = addressModelFactory;
            this._userModelFactory = userModelFactory;
            this._authenticationService = authenticationService;
            this._dateTimeSettings = dateTimeSettings;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._userService = userService;
            this._userAttributeParser = userAttributeParser;
            this._userAttributeService = userAttributeService;
            this._genericAttributeService = genericAttributeService;
            this._userRegistrationService = userRegistrationService;
            this._userSettings = userSettings;
            this._addressSettings = addressSettings;
            this._addressService = addressService;
            this._countryService = countryService;
            this._pictureService = pictureService;
            this._openAuthenticationService = openAuthenticationService;
            this._webHelper = webHelper;
            this._userActivityService = userActivityService;
            this._addressAttributeParser = addressAttributeParser;
            this._addressAttributeService = addressAttributeService;
            this._storeService = storeService;
            this._eventPublisher = eventPublisher;
            this._mediaSettings = mediaSettings;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationSettings;
            this._captchaSettings = captchaSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._emailservice = emailservice;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void TryAssociateAccountWithExternalAccount(User user)
        {
            var parameters = ExternalAuthorizerHelper.RetrieveParametersFromRoundTrip(true);
            if (parameters == null)
                return;

            if (_openAuthenticationService.AccountExists(parameters))
                return;

            _openAuthenticationService.AssociateExternalAccountWithUser(user, parameters);
        }

        [NonAction]
        protected virtual string ParseCustomUserAttributes(FormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            string attributesXml = "";
            var attributes = _userAttributeService.GetAllUserAttributes();
            foreach (var attribute in attributes)
            {
                string controlId = string.Format("user_attribute_{0}", attribute.Id);
            }

            return attributesXml;
        }

        #endregion

        #region Login / logout

        [SeagullHttpsRequirement(SslRequirement.Yes)]
        //available even when a store is closed
        [StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult Login(bool? checkoutAsGuest)
        {
            if (_workContext != null && _workContext.CurrentUser.UserRoles.Count() > 1)
                return RedirectToAction("Index", "Home", new { seagull = UrlHelperExtensions.ActionEncodedCustom(new { TransfareFromEntity = false }), area = "Admin" });
            var model = _userModelFactory.PrepareLoginModel(checkoutAsGuest);
            return View(model);
        }

        [HttpPost]
        [CaptchaValidator]
        //available even when a store is closed
        [StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult Login(LoginModel model, string returnUrl, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }
            if (ModelState.IsValid)
            {
                if (_userSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }
                var loginResult =
                    _userRegistrationService.ValidateUser(
                        _userSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);
                switch (loginResult)
                {
                    case UserLoginResults.Successful:
                        {
                            var user = _userSettings.UsernamesEnabled
                                ? _userService.GetUserByUsername(model.Username)
                                : _userService.GetUserByEmail(model.Email);


                            //sign in new user
                            _authenticationService.SignIn(user, model.RememberMe);

                            //raise event       
                            _eventPublisher.Publish(new UserLoggedinEvent(user));

                            //activity log
                            _userActivityService.InsertActivity(user, "PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"));

                            if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                            {
                                bool isMailOperator = user.UserRoles.Where(a => a.Name.Contains("Postal Operator")).Count() > 0;
                                bool isMailAdmin = user.UserRoles.Where(a => a.Name.Contains("Post department official")).Count() > 0;
                                //if (_workContext.CurrentUser.UserRoles.Where(a => a.Name.Contains("Postal Operator")).Count() > 0)
                                if (isMailOperator || isMailAdmin)
                                    return RedirectToAction("List", "MailReport", new { area = "Admin" });

                                return RedirectToAction("Index", "Home", new { seagull = UrlHelperExtensions.ActionEncodedCustom(new { TransfareFromEntity = false }), area = "Admin" });
                            }

                            return Redirect(returnUrl);
                        }
                    case UserLoginResults.UserNotExist:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.UserNotExist"));
                        break;
                    case UserLoginResults.Deleted:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case UserLoginResults.NotActive:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case UserLoginResults.NotRegistered:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
                        break;
                    case UserLoginResults.LockedOut:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.LockedOut"));
                        break;
                    case UserLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
                        break;
                }
            }



            //If we got this far, something failed, redisplay form
            model = _userModelFactory.PrepareLoginModel(model.CheckoutAsGuest);

            int userId = _workContext.CurrentUser.Id;
            bool checkPassword = _userService.GetCurrentPassword(userId).Password == model.Password ? true : false;
            bool checkUserName = _userService.GetUserById(userId).Username == model.Username ? true : false;
            if (!checkPassword || !checkUserName)
                return RedirectToAction("Login", new { ViewErrorMsg = true, Msg = _localizationService.GetResource("Admin.notExitSections") });

            return View(model);
        }

        //available even when a store is closed
        [StoreClosed(true)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult Logout()
        {
            //external authentication
            ExternalAuthorizerHelper.RemoveParameters();

            if (_workContext.OriginalUserIfImpersonated != null)
            {
                //activity log
                _userActivityService.InsertActivity(_workContext.OriginalUserIfImpersonated,
                    "Impersonation.Finished",
                    _localizationService.GetResource("ActivityLog.Impersonation.Finished.StoreOwner"),
                    _workContext.CurrentUser.Email, _workContext.CurrentUser.Id);
                _userActivityService.InsertActivity("Impersonation.Finished",
                    _localizationService.GetResource("ActivityLog.Impersonation.Finished.User"),
                    _workContext.OriginalUserIfImpersonated.Email, _workContext.OriginalUserIfImpersonated.Id);

                //logout impersonated user
                _genericAttributeService.SaveAttribute<int?>(_workContext.OriginalUserIfImpersonated,
                    SystemUserAttributeNames.ImpersonatedUserId, null);

                //redirect back to user details page (admin area)
                return this.RedirectToAction("Edit", "User",
                    new { id = _workContext.CurrentUser.Id, area = "Admin" });

            }

            //activity log
            _userActivityService.InsertActivity("PublicStore.Logout", _localizationService.GetResource("ActivityLog.PublicStore.Logout"));

            //standard logout 
            _authenticationService.SignOut();

            //raise logged out event       
            _eventPublisher.Publish(new UserLoggedOutEvent(_workContext.CurrentUser));

            //EU Cookie
            if (_storeInformationSettings.DisplayEuCookieLawWarning)
            {
                //the cookie law message should not pop up immediately after logout.
                //otherwise, the user will have to click it again...
                //and thus next visitor will not click it... so violation for that cookie law..
                //the only good solution in this case is to store a temporary variable
                //indicating that the EU cookie popup window should not be displayed on the next page open (after logout redirection to homepage)
                //but it'll be displayed for further page loads
                TempData["Seagull.IgnoreEuCookieLawWarning"] = true;
            }

            return RedirectToAction("Login", "User", new { area = "" });
        }

        #endregion

        #region Password recovery

        [SeagullHttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult PasswordRecovery()
        {
            var model = _userModelFactory.PreparePasswordRecoveryModel();
            return View(model);
        }

        [HttpPost, ActionName("PasswordRecovery")]
        [PublicAntiForgery]
        [FormValueRequired("send-email")]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult PasswordRecoverySend(PasswordRecoveryModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.GetUserByEmail(model.Email);
                if (user != null && user.Active && !user.Deleted)
                {
                    //save token and current date
                    var passwordRecoveryToken = Guid.NewGuid();
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.PasswordRecoveryToken,
                        passwordRecoveryToken.ToString());
                    DateTime? generatedDateTime = DateTime.UtcNow;
                    _genericAttributeService.SaveAttribute(user,
                        SystemUserAttributeNames.PasswordRecoveryTokenDateGenerated, generatedDateTime);

                    //send email
                    _workflowMessageService.SendUserPasswordRecoveryMessage(user,
                        _workContext.WorkingLanguage.Id);

                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailHasBeenSent");
                }
                else
                {
                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailNotFound");
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }


        [SeagullHttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult PasswordRecoveryConfirm(string token, string email)
        {
            var user = _userService.GetUserByEmail(email);
            if (user == null)
                return RedirectToRoute("HomePage");

            if (string.IsNullOrEmpty(user.GetAttribute<string>(SystemUserAttributeNames.PasswordRecoveryToken)))
            {
                return View(new PasswordRecoveryConfirmModel
                {
                    DisablePasswordChanging = true,
                    Result = _localizationService.GetResource("Account.PasswordRecovery.PasswordAlreadyHasBeenChanged")
                });
            }

            var model = _userModelFactory.PreparePasswordRecoveryConfirmModel();

            //validate token
            if (!user.IsPasswordRecoveryTokenValid(token))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.WrongToken");
            }

            //validate token expiration date
            if (user.IsPasswordRecoveryLinkExpired(_userSettings))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.LinkExpired");
            }

            return View(model);
        }

        [HttpPost, ActionName("PasswordRecoveryConfirm")]
        [PublicAntiForgery]
        [FormValueRequired("set-password")]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult PasswordRecoveryConfirmPOST(string token, string email, PasswordRecoveryConfirmModel model)
        {
            var user = _userService.GetUserByEmail(email);
            if (user == null)
                return RedirectToRoute("HomePage");

            //validate token
            if (!user.IsPasswordRecoveryTokenValid(token))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.WrongToken");
                return View(model);
            }

            //validate token expiration date
            if (user.IsPasswordRecoveryLinkExpired(_userSettings))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.LinkExpired");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var response = _userRegistrationService.ChangePassword(new ChangePasswordRequest(email,
                    false, _userSettings.DefaultPasswordFormat, model.NewPassword));
                if (response.Success)
                {
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.PasswordRecoveryToken,
                        "");

                    model.DisablePasswordChanging = true;
                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.PasswordHasBeenChanged");
                }
                else
                {
                    model.Result = response.Errors.FirstOrDefault();
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Register

        [SeagullHttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult Register()
        {
            //check whether registration is allowed
            if (_userSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            var model = new RegisterModel();
            model = _userModelFactory.PrepareRegisterModel(model, false, setDefaultValues: true);

            return View(model);
        }

        [HttpPost]
        [CaptchaValidator]
        [HoneypotValidator]
        [PublicAntiForgery]
        [ValidateInput(false)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult Register(RegisterModel model, string returnUrl, bool captchaValid, FormCollection form)
        {
            //check whether registration is allowed
            if (_userSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            if (_workContext.CurrentUser.IsRegistered())
            {
                //Already registered user. 
                _authenticationService.SignOut();

                //raise logged out event       
                _eventPublisher.Publish(new UserLoggedOutEvent(_workContext.CurrentUser));

                ////Save a new record
                //_workContext.CurrentUser = _userService.InsertGuestUser();
            }
            var user = _workContext.CurrentUser;
            user.RegisteredInStoreId = _storeContext.CurrentStore.Id;

            //custom user attributes
            var userAttributesXml = ParseCustomUserAttributes(form);
            var userAttributeWarnings = _userAttributeParser.GetAttributeWarnings(userAttributesXml);
            foreach (var error in userAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_localizationService));
            }

            if (ModelState.IsValid)
            {
                if (_userSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }

                bool isApproved = _userSettings.UserRegistrationType == UserRegistrationType.Standard;
                var registrationRequest = new UserRegistrationRequest(user,
                    model.Email,
                    _userSettings.UsernamesEnabled ? model.Username : model.Email,
                    model.Password,
                    _userSettings.DefaultPasswordFormat,
                    _storeContext.CurrentStore.Id,
                    isApproved);
                var registrationResult = _userRegistrationService.RegisterUser(registrationRequest);
                if (registrationResult.Success)
                {
                    //properties
                    if (_dateTimeSettings.AllowUsersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.TimeZoneId, model.TimeZoneId);
                    }

                    //form fields
                    if (_userSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Gender, model.Gender);
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.FirstName, model.FirstName);
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.LastName, model.LastName);
                    if (_userSettings.DateOfBirthEnabled)
                    {
                        DateTime? dateOfBirth = model.ParseDateOfBirth();
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.DateOfBirth, dateOfBirth);
                    }
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
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StateProvinceId,
                            model.StateProvinceId);
                    if (_userSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Phone, model.Phone);
                    if (_userSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Fax, model.Fax);


                    //save user attributes
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.CustomUserAttributes, userAttributesXml);

                    //login user now
                    if (isApproved)
                        _authenticationService.SignIn(user, true);

                    //associated with external account (if possible)
                    TryAssociateAccountWithExternalAccount(user);

                    //insert default address (if possible)
                    var defaultAddress = new Address
                    {
                        FirstName = user.GetAttribute<string>(SystemUserAttributeNames.FirstName),
                        LastName = user.GetAttribute<string>(SystemUserAttributeNames.LastName),
                        Email = user.Email,
                        CountryId = user.GetAttribute<int>(SystemUserAttributeNames.CountryId) > 0
                            ? (int?)user.GetAttribute<int>(SystemUserAttributeNames.CountryId)
                            : null,
                        StateProvinceId = user.GetAttribute<int>(SystemUserAttributeNames.StateProvinceId) > 0
                            ? (int?)user.GetAttribute<int>(SystemUserAttributeNames.StateProvinceId)
                            : null,
                        City = user.GetAttribute<string>(SystemUserAttributeNames.City),
                        Address1 = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress),
                        Address2 = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress2),
                        ZipPostalCode = user.GetAttribute<string>(SystemUserAttributeNames.ZipPostalCode),
                        PhoneNumber = user.GetAttribute<string>(SystemUserAttributeNames.Phone),
                        FaxNumber = user.GetAttribute<string>(SystemUserAttributeNames.Fax),
                        CreatedOnUtc = user.CreatedOnUtc
                    };
                    if (this._addressService.IsAddressValid(defaultAddress))
                    {
                        //some validation
                        if (defaultAddress.CountryId == 0)
                            defaultAddress.CountryId = null;
                        if (defaultAddress.StateProvinceId == 0)
                            defaultAddress.StateProvinceId = null;
                        //set default address
                        user.Addresses.Add(defaultAddress);
                        user.BillingAddress = defaultAddress;
                        _userService.UpdateUser(user);
                    }

                    //notifications
                    if (_userSettings.NotifyNewUserRegistration)
                        _workflowMessageService.SendUserRegisteredNotificationMessage(user,
                            _localizationSettings.DefaultAdminLanguageId);

                    //raise event       
                    _eventPublisher.Publish(new UserRegisteredEvent(user));

                    switch (_userSettings.UserRegistrationType)
                    {
                        case UserRegistrationType.EmailValidation:
                            {
                                //email validation message
                                _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.AccountActivationToken, Guid.NewGuid().ToString());
                                _workflowMessageService.SendUserEmailValidationMessage(user, _workContext.WorkingLanguage.Id);

                                //result
                                return RedirectToRoute("RegisterResult",
                                    new { resultId = (int)UserRegistrationType.EmailValidation });
                            }
                        case UserRegistrationType.AdminApproval:
                            {
                                return RedirectToRoute("RegisterResult",
                                    new { resultId = (int)UserRegistrationType.AdminApproval });
                            }
                        case UserRegistrationType.Standard:
                            {
                                //send user welcome message
                                _workflowMessageService.SendUserWelcomeMessage(user, _workContext.WorkingLanguage.Id);

                                var redirectUrl = Url.RouteUrl("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
                                if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                    redirectUrl = _webHelper.ModifyQueryString(redirectUrl, "returnurl=" + HttpUtility.UrlEncode(returnUrl), null);
                                return Redirect(redirectUrl);
                            }
                        default:
                            {
                                return RedirectToRoute("HomePage");
                            }
                    }
                }

                //errors
                foreach (var error in registrationResult.Errors)
                    ModelState.AddModelError("", error);
            }

            //If we got this far, something failed, redisplay form
            model = _userModelFactory.PrepareRegisterModel(model, true, userAttributesXml);
            return View(model);
        }

        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult RegisterResult(int resultId)
        {
            var model = _userModelFactory.PrepareRegisterResultModel(resultId);
            return View(model);
        }

        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        [HttpPost]
        public virtual ActionResult RegisterResult(string returnUrl)
        {
            if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                return RedirectToRoute("HomePage");

            return Redirect(returnUrl);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult CheckUsernameAvailability(string username)
        {
            var usernameAvailable = false;
            var statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.NotAvailable");

            if (_userSettings.UsernamesEnabled && !String.IsNullOrWhiteSpace(username))
            {
                if (_workContext.CurrentUser != null &&
                    _workContext.CurrentUser.Username != null &&
                    _workContext.CurrentUser.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                {
                    statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.CurrentUsername");
                }
                else
                {
                    var user = _userService.GetUserByUsername(username);
                    if (user == null)
                    {
                        statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.Available");
                        usernameAvailable = true;
                    }
                }
            }

            return Json(new { Available = usernameAvailable, Text = statusText });
        }

        [SeagullHttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult AccountActivation(string token, string email)
        {
            var user = _userService.GetUserByEmail(email);
            if (user == null)
                return RedirectToRoute("HomePage");

            var cToken = user.GetAttribute<string>(SystemUserAttributeNames.AccountActivationToken);
            if (string.IsNullOrEmpty(cToken))
                return
                    View(new AccountActivationModel
                    {
                        Result = _localizationService.GetResource("Account.AccountActivation.AlreadyActivated")
                    });

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("HomePage");

            //activate user account
            user.Active = true;
            _userService.UpdateUser(user);
            _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.AccountActivationToken, "");
            //send welcome message
            _workflowMessageService.SendUserWelcomeMessage(user, _workContext.WorkingLanguage.Id);

            var model = new AccountActivationModel();
            model.Result = _localizationService.GetResource("Account.AccountActivation.Activated");
            return View(model);
        }

        #endregion

        #region My account / Info

        [ChildActionOnly]
        public virtual ActionResult UserNavigation(int selectedTabId = 0)
        {
            var model = _userModelFactory.PrepareUserNavigationModel(selectedTabId);
            return PartialView(model);
        }

        [SeagullHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult Info()
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = new UserInfoModel();
            model = _userModelFactory.PrepareUserInfoModel(model, _workContext.CurrentUser, false);

            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]
        public virtual ActionResult Info(UserInfoModel model, FormCollection form)
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

            //custom user attributes
            var userAttributesXml = ParseCustomUserAttributes(form);
            var userAttributeWarnings = _userAttributeParser.GetAttributeWarnings(userAttributesXml);
            foreach (var error in userAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    //username 
                    if (_userSettings.UsernamesEnabled && this._userSettings.AllowUsersToChangeUsernames)
                    {
                        if (
                            !user.Username.Equals(model.Username.Trim(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            //change username
                            _userRegistrationService.SetUsername(user, model.Username.Trim());

                            //re-authenticate
                            //do not authenticate users in impersonation mode
                            if (_workContext.OriginalUserIfImpersonated == null)
                                _authenticationService.SignIn(user, true);
                        }
                    }
                    //email
                    if (!user.Email.Equals(model.Email.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        //change email
                        var requireValidation = _userSettings.UserRegistrationType ==
                                                UserRegistrationType.EmailValidation;
                        _userRegistrationService.SetEmail(user, model.Email.Trim(), requireValidation);

                        //do not authenticate users in impersonation mode
                        if (_workContext.OriginalUserIfImpersonated == null)
                        {
                            //re-authenticate (if usernames are disabled)
                            if (!_userSettings.UsernamesEnabled && !requireValidation)
                                _authenticationService.SignIn(user, true);
                        }
                    }

                    //properties
                    if (_dateTimeSettings.AllowUsersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.TimeZoneId,
                            model.TimeZoneId);
                    }

                    //form fields
                    if (_userSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Gender,
                            model.Gender);
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.FirstName,
                        model.FirstName);
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.LastName,
                        model.LastName);
                    if (_userSettings.DateOfBirthEnabled)
                    {
                        DateTime? dateOfBirth = model.ParseDateOfBirth();
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.DateOfBirth,
                            dateOfBirth);
                    }

                    if (_userSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StreetAddress,
                            model.StreetAddress);
                    if (_userSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StreetAddress2,
                            model.StreetAddress2);
                    if (_userSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.ZipPostalCode,
                            model.ZipPostalCode);
                    if (_userSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.City, model.City);
                    if (_userSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.CountryId,
                            model.CountryId);
                    if (_userSettings.CountryEnabled && _userSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StateProvinceId,
                            model.StateProvinceId);
                    if (_userSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Phone, model.Phone);
                    if (_userSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Fax, model.Fax);

                    //save user attributes
                    _genericAttributeService.SaveAttribute(_workContext.CurrentUser,
                        SystemUserAttributeNames.CustomUserAttributes, userAttributesXml);

                    return RedirectToRoute("UserInfo");
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }


            //If we got this far, something failed, redisplay form
            model = _userModelFactory.PrepareUserInfoModel(model, user, true, userAttributesXml);
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        public virtual ActionResult RemoveExternalAssociation(int id)
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return new HttpUnauthorizedResult();

            //ensure it's our record
            var ear = _openAuthenticationService.GetExternalIdentifiersFor(_workContext.CurrentUser)
                .FirstOrDefault(x => x.Id == id);

            if (ear == null)
            {
                return Json(new
                {
                    redirect = Url.Action("Info"),
                });
            }

            _openAuthenticationService.DeleteExternalAuthenticationRecord(ear);

            return Json(new
            {
                redirect = Url.Action("Info"),
            });
        }

        [SeagullHttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult EmailRevalidation(string token, string email)
        {
            var user = _userService.GetUserByEmail(email);
            if (user == null)
                return RedirectToRoute("HomePage");

            var cToken = user.GetAttribute<string>(SystemUserAttributeNames.EmailRevalidationToken);
            if (string.IsNullOrEmpty(cToken))
                return View(new EmailRevalidationModel
                {
                    Result = _localizationService.GetResource("Account.EmailRevalidation.AlreadyChanged")
                });

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("HomePage");

            if (String.IsNullOrEmpty(user.EmailToRevalidate))
                return RedirectToRoute("HomePage");

            if (_userSettings.UserRegistrationType != UserRegistrationType.EmailValidation)
                return RedirectToRoute("HomePage");

            //change email
            try
            {
                _userRegistrationService.SetEmail(user, user.EmailToRevalidate, false);
            }
            catch (Exception exc)
            {
                return View(new EmailRevalidationModel
                {
                    Result = _localizationService.GetResource(exc.Message)
                });
            }
            user.EmailToRevalidate = null;
            _userService.UpdateUser(user);
            _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.EmailRevalidationToken, "");

            //re-authenticate (if usernames are disabled)
            if (!_userSettings.UsernamesEnabled)
            {
                _authenticationService.SignIn(user, true);
            }

            var model = new EmailRevalidationModel()
            {
                Result = _localizationService.GetResource("Account.EmailRevalidation.Changed")
            };
            return View(model);
        }

        #endregion

        #region My account / Addresses

        [SeagullHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult Addresses()
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = _userModelFactory.PrepareUserAddressListModel();
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [SeagullHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult AddressDelete(int addressId)
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

            //find address (ensure that it belongs to the current user)
            var address = user.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address != null)
            {
                user.RemoveAddress(address);
                _userService.UpdateUser(user);
                //now delete the address record
                _addressService.DeleteAddress(address);
            }

            //redirect to the address list page
            return Json(new
            {
                redirect = Url.RouteUrl("UserAddresses"),
            });
        }

        [SeagullHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult AddressAdd()
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = new UserAddressEditModel();
            _addressModelFactory.PrepareAddressModel(model.Address,
                address: null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));

            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]
        public virtual ActionResult AddressAdd(UserAddressEditModel model, FormCollection form)
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

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

                return RedirectToRoute("UserAddresses");
            }

            //If we got this far, something failed, redisplay form
            _addressModelFactory.PrepareAddressModel(model.Address,
                address: null,
                excludeProperties: true,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id),
                overrideAttributesXml: customAttributes);

            return View(model);
        }

        [SeagullHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult AddressEdit(int addressId)
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;
            //find address (ensure that it belongs to the current user)
            var address = user.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                //address is not found
                return RedirectToRoute("UserAddresses");

            var model = new UserAddressEditModel();
            _addressModelFactory.PrepareAddressModel(model.Address,
                address: address,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));

            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        [ValidateInput(false)]
        public virtual ActionResult AddressEdit(UserAddressEditModel model, int addressId, FormCollection form)
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;
            //find address (ensure that it belongs to the current user)
            var address = user.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                //address is not found
                return RedirectToRoute("UserAddresses");

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

                return RedirectToRoute("UserAddresses");
            }

            //If we got this far, something failed, redisplay form
            _addressModelFactory.PrepareAddressModel(model.Address,
                address: address,
                excludeProperties: true,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id),
                overrideAttributesXml: customAttributes);
            return View(model);
        }

        #endregion

        #region My account / Change password

        [SeagullHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult ChangePassword()
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = _userModelFactory.PrepareChangePasswordModel();

            ////display the cause of the change password 
            //if (_workContext.CurrentUser.PasswordIsExpired())
            //    ModelState.AddModelError(string.Empty, _localizationService.GetResource("Account.ChangePassword.PasswordIsExpired"));

            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        public virtual ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(user.Email,
                    true, _userSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
                var changePasswordResult = _userRegistrationService.ChangePassword(changePasswordRequest);
                if (changePasswordResult.Success)
                {
                    model.Result = _localizationService.GetResource("Account.ChangePassword.Success");
                    return View(model);
                }

                //errors
                foreach (var error in changePasswordResult.Errors)
                    ModelState.AddModelError("", error);
            }


            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region My account / Avatar

        [SeagullHttpsRequirement(SslRequirement.Yes)]
        public virtual ActionResult Avatar()
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_userSettings.AllowUsersToUploadAvatars)
                return RedirectToRoute("UserInfo");

            var model = new UserAvatarModel();
            model = _userModelFactory.PrepareUserAvatarModel(model);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [PublicAntiForgery]
        [FormValueRequired("upload-avatar")]
        public virtual ActionResult UploadAvatar(UserAvatarModel model, HttpPostedFileBase uploadedFile)
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_userSettings.AllowUsersToUploadAvatars)
                return RedirectToRoute("UserInfo");

            var user = _workContext.CurrentUser;

            if (ModelState.IsValid)
            {
                try
                {
                    var userAvatar = _pictureService.GetPictureById(user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId));
                    if ((uploadedFile != null) && (!String.IsNullOrEmpty(uploadedFile.FileName)))
                    {
                        int avatarMaxSize = _userSettings.AvatarMaximumSizeBytes;
                        if (uploadedFile.ContentLength > avatarMaxSize)
                            throw new SeagullException(string.Format(_localizationService.GetResource("Account.Avatar.MaximumUploadedFileSize"), avatarMaxSize));

                        byte[] userPictureBinary = uploadedFile.GetPictureBits();
                        if (userAvatar != null)
                            userAvatar = _pictureService.UpdatePicture(userAvatar.Id, userPictureBinary, uploadedFile.ContentType, null);
                        else
                            userAvatar = _pictureService.InsertPicture(userPictureBinary, uploadedFile.ContentType, null);
                    }

                    int userAvatarId = 0;
                    if (userAvatar != null)
                        userAvatarId = userAvatar.Id;

                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.AvatarPictureId, userAvatarId);

                    model.AvatarUrl = _pictureService.GetPictureUrl(
                        user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId),
                        _mediaSettings.AvatarPictureSize,
                        false);
                    return View(model);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }


            //If we got this far, something failed, redisplay form
            model = _userModelFactory.PrepareUserAvatarModel(model);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [PublicAntiForgery]
        [FormValueRequired("remove-avatar")]
        public virtual ActionResult RemoveAvatar(UserAvatarModel model, HttpPostedFileBase uploadedFile)
        {
            if (!_workContext.CurrentUser.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_userSettings.AllowUsersToUploadAvatars)
                return RedirectToRoute("UserInfo");

            var user = _workContext.CurrentUser;

            var userAvatar = _pictureService.GetPictureById(user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId));
            if (userAvatar != null)
                _pictureService.DeletePicture(userAvatar);
            _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.AvatarPictureId, 0);

            return RedirectToRoute("UserAvatar");
        }

        #endregion
    }
}
