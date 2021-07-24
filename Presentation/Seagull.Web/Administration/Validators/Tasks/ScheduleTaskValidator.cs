using FluentValidation;
using Seagull.Admin.Models.Tasks;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;

namespace Seagull.Admin.Validators.Tasks
{
    public partial class ScheduleTaskValidator : BaseSeagullValidator<ScheduleTaskModel>
    {
        public ScheduleTaskValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.System.ScheduleTasks.Name.Required"));
            RuleFor(x => x.Seconds).GreaterThan(0).WithMessage(localizationService.GetResource("Admin.System.ScheduleTasks.Seconds.Positive"));
        }
    }
}