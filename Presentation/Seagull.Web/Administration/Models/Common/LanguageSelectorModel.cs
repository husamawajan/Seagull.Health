using System.Collections.Generic;
using Seagull.Admin.Models.Localization;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Common
{
    public partial class LanguageSelectorModel : BaseSeagullModel
    {
        public LanguageSelectorModel()
        {
            AvailableLanguages = new List<LanguageModel>();
        }

        public IList<LanguageModel> AvailableLanguages { get; set; }

        public LanguageModel CurrentLanguage { get; set; }
    }
}