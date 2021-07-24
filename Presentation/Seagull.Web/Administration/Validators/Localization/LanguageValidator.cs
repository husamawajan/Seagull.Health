using System.Globalization;
using FluentValidation;
using Seagull.Admin.Models.Localization;
using Seagull.Core.Domain.Localization;
using Seagull.Data;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Validators;

namespace Seagull.Admin.Validators.Localization
{
    public partial class LanguageValidator : BaseSeagullValidator<LanguageModel>
    {
        public LanguageValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Languages.Fields.Name.Required"));
            RuleFor(x => x.LanguageCulture)
                .Must(x =>
                          {
                              try
                              {
                                  //let's try to create a CultureInfo object
                                  //if "DisplayLocale" is wrong, then exception will be thrown
                                  var culture = new CultureInfo(x);
                                  return true;
                              }
                              catch
                              {
                                  return false;
                              }
                          })
                .WithMessage(localizationService.GetResource("Admin.Configuration.Languages.Fields.LanguageCulture.Validation"));

            RuleFor(x => x.UniqueSeoCode).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Languages.Fields.UniqueSeoCode.Required"));
            RuleFor(x => x.UniqueSeoCode).Length(2).WithMessage(localizationService.GetResource("Admin.Configuration.Languages.Fields.UniqueSeoCode.Length"));

            SetDatabaseValidationRules<Language>(dbContext, "UniqueSeoCode");

        }
    }
}