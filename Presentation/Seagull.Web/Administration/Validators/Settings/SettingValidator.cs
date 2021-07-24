using FluentValidation;
using Seagull.Admin.Models.Settings;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;

namespace Seagull.Admin.Validators.Settings
{
    public partial class SettingValidator : BaseSeagullValidator<SettingModel>
    {
        public SettingValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Settings.AllSettings.Fields.Name.Required"));
        }
    }
}