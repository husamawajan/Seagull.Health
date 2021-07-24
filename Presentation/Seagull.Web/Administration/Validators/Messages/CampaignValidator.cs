using FluentValidation;
using Seagull.Admin.Models.Messages;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;

namespace Seagull.Admin.Validators.Messages
{
    public partial class CampaignValidator : BaseSeagullValidator<CampaignModel>
    {
        public CampaignValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Promotions.Campaigns.Fields.Name.Required"));

            RuleFor(x => x.Subject).NotEmpty().WithMessage(localizationService.GetResource("Admin.Promotions.Campaigns.Fields.Subject.Required"));

            RuleFor(x => x.Body).NotEmpty().WithMessage(localizationService.GetResource("Admin.Promotions.Campaigns.Fields.Body.Required"));
        }
    }
}