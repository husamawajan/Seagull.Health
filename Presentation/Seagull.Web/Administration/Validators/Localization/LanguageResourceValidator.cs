using FluentValidation;
using Seagull.Admin.Models.Localization;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;

namespace Seagull.Admin.Validators.Localization
{
    public partial class LanguageResourceValidator : BaseSeagullValidator<LanguageResourceModel>
    {
        public LanguageResourceValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Languages.Resources.Fields.Name.Required"));
            RuleFor(x => x.Value).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Languages.Resources.Fields.Value.Required"));
        }
    }
}