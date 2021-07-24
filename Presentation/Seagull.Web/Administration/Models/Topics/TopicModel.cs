using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Localization;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Topics
{
    public partial class TopicModel : BaseSeagullEntityModel, ILocalizedModel<TopicLocalizedModel>
    {
        public TopicModel()
        {
            AvailableTopicTemplates = new List<SelectListItem>();
            Locales = new List<TopicLocalizedModel>();

            SelectedUserRoleIds = new List<int>();
            AvailableUserRoles = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.SystemName")]
        [AllowHtml]
        public string SystemName { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInSitemap")]
        public bool IncludeInSitemap { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInTopMenu")]
        public bool IncludeInTopMenu { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn1")]
        public bool IncludeInFooterColumn1 { get; set; }
        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn2")]
        public bool IncludeInFooterColumn2 { get; set; }
        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.IncludeInFooterColumn3")]
        public bool IncludeInFooterColumn3 { get; set; }
        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.AccessibleWhenStoreClosed")]
        public bool AccessibleWhenStoreClosed { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.IsPasswordProtected")]
        public bool IsPasswordProtected { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.Password")]
        public string Password { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.URL")]
        [AllowHtml]
        public string Url { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.Title")]
        [AllowHtml]
        public string Title { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.Published")]
        public bool Published { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.TopicTemplate")]
        public int TopicTemplateId { get; set; }
        public IList<SelectListItem> AvailableTopicTemplates { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }

        public IList<TopicLocalizedModel> Locales { get; set; }

        //store mapping
        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.LimitedToStores")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        //ACL (user roles)
        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.AclUserRoles")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedUserRoleIds { get; set; }
        public IList<SelectListItem> AvailableUserRoles { get; set; }
        public string EncId { get; set; } //Enrcypt Url

    }

    public partial class TopicLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.Title")]
        [AllowHtml]
        public string Title { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }
        
    }
}