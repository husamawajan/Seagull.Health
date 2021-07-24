using FluentValidation;
using Seagull.Core.Domain.Users;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;
using Seagull.Web.Models.User;

namespace Seagull.Web.Validators.User
{
    public partial class LoginValidator : BaseSeagullValidator<LoginModel>
    {
        public LoginValidator(ILocalizationService localizationService, UserSettings userSettings)
        {
            if (!userSettings.UsernamesEnabled)
            {
                //login by email
                RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.Login.Fields.Email.Required"));
                RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
            }
        }
    }
}