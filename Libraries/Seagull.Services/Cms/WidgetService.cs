using System;
using System.Collections.Generic;
using System.Linq;
using Seagull.Core.Domain.Cms;
using Seagull.Core.Domain.Users;
using Seagull.Core.Plugins;

namespace Seagull.Services.Cms
{
    /// <summary>
    /// Widget service
    /// </summary>
    public partial class WidgetService : IWidgetService
    {
        #region Fields

        private readonly IPluginFinder _pluginFinder;
        private readonly WidgetSettings _widgetSettings;

        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pluginFinder">Plugin finder</param>
        /// <param name="widgetSettings">Widget settings</param>
        public WidgetService(IPluginFinder pluginFinder,
            WidgetSettings widgetSettings)
        {
            this._pluginFinder = pluginFinder;
            this._widgetSettings = widgetSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load active widgets
        /// </summary>
        /// <param name="user">Load records allowed only to a specified user; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Widgets</returns>
        public virtual IList<IWidgetPlugin> LoadActiveWidgets(User user = null, int storeId = 0)
        {
            return LoadAllWidgets(user, storeId)
                .Where(x => _widgetSettings.ActiveWidgetSystemNames.Contains(x.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase)).ToList();
        }

        /// <summary>
        /// Load active widgets
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <param name="user">Load records allowed only to a specified user; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Widgets</returns>
        public virtual IList<IWidgetPlugin> LoadActiveWidgetsByWidgetZone(string  widgetZone, User user = null, int storeId = 0)
        {
            if (String.IsNullOrWhiteSpace(widgetZone))
                return new List<IWidgetPlugin>();

            return LoadActiveWidgets(user, storeId)
                .Where(x => x.GetWidgetZones().Contains(widgetZone, StringComparer.InvariantCultureIgnoreCase)).ToList();
        }

        /// <summary>
        /// Load widget by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found widget</returns>
        public virtual IWidgetPlugin LoadWidgetBySystemName(string systemName)
        {
            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<IWidgetPlugin>(systemName);
            if (descriptor != null)
                return descriptor.Instance<IWidgetPlugin>();

            return null;
        }

        /// <summary>
        /// Load all widgets
        /// </summary>
        /// <param name="user">Load records allowed only to a specified user; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Widgets</returns>
        public virtual IList<IWidgetPlugin> LoadAllWidgets(User user = null, int storeId = 0)
        {
            return _pluginFinder.GetPlugins<IWidgetPlugin>(user: user, storeId: storeId).ToList();
        }
        
        #endregion
    }
}
