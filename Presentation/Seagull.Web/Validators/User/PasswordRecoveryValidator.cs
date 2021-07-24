using FluentValidation;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;
using Seagull.Web.Models.User;

namespace Seagull.Web.Validators.User
{
    public partial class PasswordRecoveryValidator : BaseSeagullValidator<PasswordRecoveryModel>
    {
        public PasswordRecoveryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.PasswordRecovery.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
        }}
}