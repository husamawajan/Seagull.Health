using FluentValidation;
using Seagull.Admin.Models.Messages;
using Seagull.Core.Domain.Messages;
using Seagull.Data;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;

namespace Seagull.Admin.Validators.Messages
{
    public partial class EmailAccountValidator : BaseSeagullValidator<EmailAccountModel>
    {
        public EmailAccountValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"));
            
            RuleFor(x => x.DisplayName).NotEmpty();

            SetDatabaseValidationRules<EmailAccount>(dbContext);
        }
    }
}