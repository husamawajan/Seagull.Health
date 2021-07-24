using System.Globalization;
using FluentValidation;
using Seagull.Admin.Models.Functionss;
using Seagull.Services.Functionss;
using Seagull.Admin.Models.Localization;
using Seagull.Services.Localization;

namespace Seagull.Admin.Validators.Functionss
{
    public class FunctionsValidator : AbstractValidator<FunctionsModel>
    {
        public FunctionsValidator(ILocalizationService localizationService)
        {
        }
    }
}
	

