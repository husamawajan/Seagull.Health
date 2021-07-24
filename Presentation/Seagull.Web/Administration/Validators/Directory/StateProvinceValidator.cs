using FluentValidation;
using Seagull.Admin.Models.Directory;
using Seagull.Core.Domain.Directory;
using Seagull.Data;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;

namespace Seagull.Admin.Validators.Directory
{
    public partial class StateProvinceValidator : BaseSeagullValidator<StateProvinceModel>
    {
        public StateProvinceValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Countries.States.Fields.Name.Required"));

            SetDatabaseValidationRules<StateProvince>(dbContext);
        }
    }
}