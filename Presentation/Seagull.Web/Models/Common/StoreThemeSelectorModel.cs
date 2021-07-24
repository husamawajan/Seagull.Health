using System.Collections.Generic;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Web.Models.Common
{
    public partial class StoreThemeSelectorModel : BaseSeagullModel
    {
        public StoreThemeSelectorModel()
        {
            AvailableStoreThemes = new List<StoreThemeModel>();
        }

        public IList<StoreThemeModel> AvailableStoreThemes { get; set; }

        public StoreThemeModel CurrentStoreTheme { get; set; }
    }
}