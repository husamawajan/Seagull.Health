using FluentValidation;
using Seagull.Admin.Models.Stores;
using Seagull.Core.Domain.Stores;
using Seagull.Data;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;

namespace Seagull.Admin.Validators.Stores
{
    public partial class StoreValidator : BaseSeagullValidator<StoreModel>
    {
        public StoreValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Stores.Fields.Name.Required"));
            RuleFor(x => x.Url).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Stores.Fields.Url.Required"));

            SetDatabaseValidationRules<Store>(dbContext);
        }
    }
}