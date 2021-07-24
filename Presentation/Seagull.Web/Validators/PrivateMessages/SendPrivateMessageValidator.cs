using FluentValidation;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;
using Seagull.Web.Models.PrivateMessages;

namespace Seagull.Web.Validators.PrivateMessages
{
    public partial class SendPrivateMessageValidator : BaseSeagullValidator<SendPrivateMessageModel>
    {
        public SendPrivateMessageValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Subject).NotEmpty().WithMessage(localizationService.GetResource("PrivateMessages.SubjectCannotBeEmpty"));
            RuleFor(x => x.Message).NotEmpty().WithMessage(localizationService.GetResource("PrivateMessages.MessageCannotBeEmpty"));
        }
    }
}