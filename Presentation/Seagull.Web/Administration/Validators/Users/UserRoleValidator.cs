using FluentValidation;
using Seagull.Admin.Models.Users;
using Seagull.Core.Domain.Users;
using Seagull.Data;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;

namespace Seagull.Admin.Validators.Users
{
    public partial class UserRoleValidator : BaseSeagullValidator<UserRoleModel>
    {
        public UserRoleValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Users.UserRoles.Fields.Name.Required"));

            SetDatabaseValidationRules<UserRole>(dbContext);
        }
    }
}