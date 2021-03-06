using System;
using System.Linq;
using Seagull.Core;
using Seagull.Core.Domain;
using Seagull.Core.Domain.Users;
using Seagull.Services.Common;

namespace Seagull.Web.Framework.Themes
{
    /// <summary>
    /// Theme context
    /// </summary>
    public partial class ThemeContext : IThemeContext
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly IThemeProvider _themeProvider;

        private bool _themeIsCached;
        private string _cachedThemeName;

        public ThemeContext(IWorkContext workContext,
            IStoreContext storeContext,
            IGenericAttributeService genericAttributeService, 
            StoreInformationSettings storeInformationSettings, 
            IThemeProvider themeProvider)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._genericAttributeService = genericAttributeService;
            this._storeInformationSettings = storeInformationSettings;
            this._themeProvider = themeProvider;
        }

        /// <summary>
        /// Get or set current theme system name
        /// </summary>
        public string WorkingThemeName
        {
            get
            {
                if (_themeIsCached)
                    return _cachedThemeName;

                string theme = "";
                if (_storeInformationSettings.AllowUserToSelectTheme)
                {
                    if (_workContext.CurrentUser != null)
                        theme = _workContext.CurrentUser.GetAttribute<string>(SystemUserAttributeNames.WorkingThemeName, _genericAttributeService, _storeContext.CurrentStore.Id);
                }

                //default store theme
                if (string.IsNullOrEmpty(theme))
                    theme = _storeInformationSettings.DefaultStoreTheme;

                //ensure that theme exists
                if (!_themeProvider.ThemeConfigurationExists(theme))
                {
                    var themeInstance = _themeProvider.GetThemeConfigurations()
                        .FirstOrDefault();
                    if (themeInstance == null)
                        throw new Exception("No theme could be loaded");
                    theme = themeInstance.ThemeName;
                }
                
                //cache theme
                this._cachedThemeName = theme;
                this._themeIsCached = true;
                return theme;
            }
            set
            {
                if (!_storeInformationSettings.AllowUserToSelectTheme)
                    return;

                if (_workContext.CurrentUser == null)
                    return;

                _genericAttributeService.SaveAttribute(_workContext.CurrentUser, SystemUserAttributeNames.WorkingThemeName, value, _storeContext.CurrentStore.Id);

                //clear cache
                this._themeIsCached = false;
            }
        }
    }
}
