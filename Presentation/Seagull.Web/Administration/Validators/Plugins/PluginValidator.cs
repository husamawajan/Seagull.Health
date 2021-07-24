using FluentValidation;
using Seagull.Admin.Models.Plugins;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;

namespace Seagull.Admin.Validators.Plugins
{
    public partial class PluginValidator : BaseSeagullValidator<PluginModel>
    {
        public PluginValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FriendlyName).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Plugins.Fields.FriendlyName.Required"));
        }
    }
}