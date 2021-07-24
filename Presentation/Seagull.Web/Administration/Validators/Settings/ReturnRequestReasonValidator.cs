using FluentValidation;
using Seagull.Admin.Models.Settings;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;

namespace Seagull.Admin.Validators.Settings
{
    public partial class ReturnRequestReasonValidator : BaseSeagullValidator<ReturnRequestReasonModel>
    {
        public ReturnRequestReasonValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestReasons.Name.Required"));
        }
    }
}