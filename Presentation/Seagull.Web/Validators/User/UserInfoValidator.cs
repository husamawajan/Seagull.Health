using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Seagull.Core;
using Seagull.Core.Domain.Users;
using Seagull.Services.Directory;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;
using Seagull.Web.Models.User;

namespace Seagull.Web.Validators.User
{
    public partial class UserInfoValidator : BaseSeagullValidator<UserInfoModel>
    {
        public UserInfoValidator(ILocalizationService localizationService,
            IStateProvinceService stateProvinceService, 
            UserSettings userSettings)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.FirstName.Required"));
            RuleFor(x => x.LastName).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.LastName.Required"));

            if (userSettings.UsernamesEnabled && userSettings.AllowUsersToChangeUsernames)
            {
                RuleFor(x => x.Username).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Username.Required"));
            }

            //form fields
            if (userSettings.CountryEnabled && userSettings.CountryRequired)
            {
                RuleFor(x => x.CountryId)
                    .NotEqual(0)
                    .WithMessage(localizationService.GetResource("Account.Fields.Country.Required"));
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
            if (userSettings.DateOfBirthEnabled &&userSettings.DateOfBirthRequired)
            {
                Custom(x =>
                {
                    var dateOfBirth = x.ParseDateOfBirth();
                    //entered?
                    if (!dateOfBirth.HasValue)
                    {
                        return new ValidationFailure("DateOfBirthDay", localizationService.GetResource("Account.Fields.DateOfBirth.Required"));
                    }
                    //minimum age
                    if (userSettings.DateOfBirthMinimumAge.HasValue &&
                        CommonHelper.GetDifferenceInYears(dateOfBirth.Value, DateTime.Today) < userSettings.DateOfBirthMinimumAge.Value)
                    {
                        return new ValidationFailure("DateOfBirthDay", string.Format(localizationService.GetResource("Account.Fields.DateOfBirth.MinimumAge"), userSettings.DateOfBirthMinimumAge.Value));
                    }
                    return null;
                });
            }
           
            if (userSettings.StreetAddressRequired && userSettings.StreetAddressEnabled)
            {
                RuleFor(x => x.StreetAddress).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.StreetAddress.Required"));
            }
            if (userSettings.StreetAddress2Required && userSettings.StreetAddress2Enabled)
            {
                RuleFor(x => x.StreetAddress2).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.StreetAddress2.Required"));
            }
            if (userSettings.ZipPostalCodeRequired && userSettings.ZipPostalCodeEnabled)
            {
                RuleFor(x => x.ZipPostalCode).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.ZipPostalCode.Required"));
            }
            if (userSettings.CityRequired && userSettings.CityEnabled)
            {
                RuleFor(x => x.City).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.City.Required"));
            }
            if (userSettings.PhoneRequired && userSettings.PhoneEnabled)
            {
                RuleFor(x => x.Phone).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Phone.Required"));
            }
            if (userSettings.FaxRequired && userSettings.FaxEnabled)
            {
                RuleFor(x => x.Fax).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Fax.Required"));
            }
        }
    }
}