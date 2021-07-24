using FluentValidation;
using Seagull.Admin.Models.Messages;
using Seagull.Core.Domain.Messages;
using Seagull.Data;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;

namespace Seagull.Admin.Validators.Messages
{
    public partial class QueuedEmailValidator : BaseSeagullValidator<QueuedEmailModel>
    {
        public QueuedEmailValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.From).NotEmpty().WithMessage(localizationService.GetResource("Admin.System.QueuedEmails.Fields.From.Required"));
            RuleFor(x => x.To).NotEmpty().WithMessage(localizationService.GetResource("Admin.System.QueuedEmails.Fields.To.Required"));

            RuleFor(x => x.SentTries).NotNull().WithMessage(localizationService.GetResource("Admin.System.QueuedEmails.Fields.SentTries.Required"))
                                    .InclusiveBetween(0, 99999).WithMessage(localizationService.GetResource("Admin.System.QueuedEmails.Fields.SentTries.Range"));

            SetDatabaseValidationRules<QueuedEmail>(dbContext);

        }
    }
}