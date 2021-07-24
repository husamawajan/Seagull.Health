using FluentValidation;
using Seagull.Admin.Models.Common;
using Seagull.Core.Domain.Common;
using Seagull.Data;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;

namespace Seagull.Admin.Validators.Common
{
    public partial class AddressAttributeValueValidator : BaseSeagullValidator<AddressAttributeValueModel>
    {
        public AddressAttributeValueValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Address.AddressAttributes.Values.Fields.Name.Required"));

            SetDatabaseValidationRules<AddressAttributeValue>(dbContext);
        }
    }
}