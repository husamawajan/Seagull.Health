using FluentValidation;
using Seagull.Admin.Models.Messages;
using Seagull.Core.Domain.Messages;
using Seagull.Data;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;

namespace Seagull.Admin.Validators.Messages
{
    public partial class MessageTemplateValidator : BaseSeagullValidator<MessageTemplateModel>
    {
        public MessageTemplateValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Subject).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Fields.Subject.Required"));
            RuleFor(x => x.Body).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Fields.Body.Required"));

            SetDatabaseValidationRules<MessageTemplate>(dbContext);
        }
    }
}