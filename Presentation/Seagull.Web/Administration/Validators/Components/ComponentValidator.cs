using System.Globalization;
using FluentValidation;
using Seagull.Admin.Models.Components;
using Seagull.Services.Components;
using Seagull.Admin.Models.Localization;
using Seagull.Services.Localization;

namespace Seagull.Admin.Validators.Components
{
    public class ComponentValidator : AbstractValidator<ComponentModel>
    {
        public ComponentValidator(ILocalizationService localizationService)
        {
        }
    }
}
