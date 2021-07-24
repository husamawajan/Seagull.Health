using FluentValidation;
using Seagull.Admin.Models.Messages;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;

namespace Seagull.Admin.Validators.Messages
{
    public partial class TestMessageTemplateValidator : BaseSeagullValidator<TestMessageTemplateModel>
    {
        public TestMessageTemplateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SendTo).NotEmpty();
            RuleFor(x => x.SendTo).EmailAddress().WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"));
        }
    }
}