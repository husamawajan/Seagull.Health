using System.Collections.Generic;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Web.Models.Common
{
    public partial class LanguageSelectorModel : BaseSeagullModel
    {
        public LanguageSelectorModel()
        {
            AvailableLanguages = new List<LanguageModel>();
        }

        public IList<LanguageModel> AvailableLanguages { get; set; }

        public int CurrentLanguageId { get; set; }

        public bool UseImages { get; set; }
    }
}