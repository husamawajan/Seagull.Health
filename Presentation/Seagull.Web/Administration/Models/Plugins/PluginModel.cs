using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Plugins;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Localization;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Plugins
{
    [Validator(typeof(PluginValidator))]
    public partial class PluginModel : BaseSeagullModel, ILocalizedModel<PluginLocalizedModel>
    {
        public PluginModel()
        {
            Locales = new List<PluginLocalizedModel>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
            SelectedUserRoleIds = new List<int>();
            AvailableUserRoles = new List<SelectListItem>();
        }
        [SeagullResourceDisplayName("Admin.Configuration.Plugins.Fields.Group")]
        [AllowHtml]
        public string Group { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Plugins.Fields.FriendlyName")]
        [AllowHtml]
        public string FriendlyName { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Plugins.Fields.SystemName")]
        [AllowHtml]
        public string SystemName { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Plugins.Fields.Version")]
        [AllowHtml]
        public string Version { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Plugins.Fields.Author")]
        [AllowHtml]
        public string Author { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Plugins.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Plugins.Fields.Configure")]
        public string ConfigurationUrl { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Plugins.Fields.Installed")]
        public bool Installed { get; set; }
        
        [AllowHtml]
        public string Description { get; set; }

        public bool CanChangeEnabled { get; set; }
        [SeagullResourceDisplayName("Admin.Configuration.Plugins.Fields.IsEnabled")]
        public bool IsEnabled { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Plugins.Fields.Logo")]
        public string LogoUrl { get; set; }

        public IList<PluginLocalizedModel> Locales { get; set; }

        //ACL (user roles)
        [SeagullResourceDisplayName("Admin.Configuration.Plugins.Fields.AclUserRoles")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedUserRoleIds { get; set; }
        public IList<SelectListItem> AvailableUserRoles { get; set; }

        //store mapping
        [SeagullResourceDisplayName("Admin.Configuration.Plugins.Fields.LimitedToStores")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }
    public partial class PluginLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Plugins.Fields.FriendlyName")]
        [AllowHtml]
        public string FriendlyName { get; set; }
    }
}