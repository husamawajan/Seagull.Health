using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Seagull.Admin.Models.Users;
using Seagull.Core.Domain.Users;
using Seagull.Data;
using Seagull.Services.Users;
using Seagull.Services.Directory;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;

namespace Seagull.Admin.Validators.Users
{
    public partial class UserValidator : BaseSeagullValidator<UserModel>
    {
        public UserValidator(ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            IUserService userService,
            UserSettings userSettings,
            IDbContext dbContext)
        {
            //ensure that valid email address is entered if Registered role is checked to avoid registered users with empty email address
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                //.WithMessage("Valid Email is required for user to be in 'Registered' role")
                .WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"))
                //only for registered users
                .When(x => IsRegisteredUserRoleChecked(x, userService));

            //form fields
            if (userSettings.CountryEnabled && userSettings.CountryRequired)
            {
                RuleFor(x => x.CountryId)
                    .NotEqual(0)
                    .WithMessage(localizationService.GetResource("Account.Fields.Country.Required"))
                    //only for registered users
                    .When(x => IsRegisteredUserRoleChecked(x, userService));
            }
            if (userSettings.CountryEnabled &&
                userSettings.StateProvinceEnabled &&
                userSettings.StateProvinceRequired)
            {
                Custom(x =>
                {
                    //does selected country have states?
                    var hasStates = stateProvinceService.GetStateProvincesByCountryId(x.CountryId).Any();
                    if (hasStates)
                    {
                        //if yes, then ensure that a state is selected
                        if (x.StateProvinceId == 0)
                        {
                            return new ValidationFailure("StateProvinceId", localizationService.GetResource("Account.Fields.StateProvince.Required"));
                        }
                    }
                    return null;
                });
            }
            
            if (userSettings.StreetAddressRequired && userSettings.StreetAddressEnabled)
            {
                RuleFor(x => x.StreetAddress)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.StreetAddress.Required"))
                    //only for registered users
                    .When(x => IsRegisteredUserRoleChecked(x, userService));
            }
            if (userSettings.StreetAddress2Required && userSettings.StreetAddress2Enabled)
            {
                RuleFor(x => x.StreetAddress2)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.StreetAddress2.Required"))
                    //only for registered users
                    .When(x => IsRegisteredUserRoleChecked(x, userService));
            }
            if (userSettings.ZipPostalCodeRequired && userSettings.ZipPostalCodeEnabled)
            {
                RuleFor(x => x.ZipPostalCode)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.ZipPostalCode.Required"))
                    //only for registered users
                    .When(x => IsRegisteredUserRoleChecked(x, userService));
            }
            if (userSettings.CityRequired && userSettings.CityEnabled)
            {
                RuleFor(x => x.City)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.City.Required"))
                    //only for registered users
                    .When(x => IsRegisteredUserRoleChecked(x, userService));
            }
            if (userSettings.PhoneRequired && userSettings.PhoneEnabled)
            {
                RuleFor(x => x.Phone)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.Phone.Required"))
                    //only for registered users
                    .When(x => IsRegisteredUserRoleChecked(x, userService));
            }
            if (userSettings.FaxRequired && userSettings.FaxEnabled)
            {
                RuleFor(x => x.Fax)
                    .NotEmpty()
                    .WithMessage(localizationService.GetResource("Admin.Users.Users.Fields.Fax.Required"))
                    //only for registered users
                    .When(x => IsRegisteredUserRoleChecked(x, userService));
            }

            SetDatabaseValidationRules<User>(dbContext);
        }

        private bool IsRegisteredUserRoleChecked(UserModel model, IUserService userService)
        {
            var allUserRoles = userService.GetAllUserRoles(true);
            var newUserRoles = new List<UserRole>();
            foreach (var userRole in allUserRoles)
                if (model.SelectedUserRoleIds.Contains(userRole.Id))
                    newUserRoles.Add(userRole);

            bool isInRegisteredRole = newUserRoles.FirstOrDefault(cr => cr.SystemName == SystemUserRoleNames.Registered) != null;
            return isInRegisteredRole;
        }
    }
}