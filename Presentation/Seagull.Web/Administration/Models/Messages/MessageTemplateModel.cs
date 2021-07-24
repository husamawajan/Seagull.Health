using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Messages;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Localization;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Messages
{
    [Validator(typeof(MessageTemplateValidator))]
    public partial class MessageTemplateModel : BaseSeagullEntityModel, ILocalizedModel<MessageTemplateLocalizedModel>
    {
        public MessageTemplateModel()
        {
            Locales = new List<MessageTemplateLocalizedModel>();
            AvailableEmailAccounts = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }


        [SeagullResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AllowedTokens")]
        public string AllowedTokens { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.BccEmailAddresses")]
        [AllowHtml]
        public string BccEmailAddresses { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.IsActive")]
        [AllowHtml]
        public bool IsActive { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.SendImmediately")]
        public bool SendImmediately { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.DelayBeforeSend")]
        [UIHint("Int32Nullable")]
        public int? DelayBeforeSend { get; set; }
        public int DelayPeriodId { get; set; }

        public bool HasAttachedDownload { get; set; }
        [SeagullResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AttachedDownload")]
        [UIHint("Download")]
        public int AttachedDownloadId { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount")]
        public int EmailAccountId { get; set; }
        public IList<SelectListItem> AvailableEmailAccounts { get; set; }

        //store mapping
        [SeagullResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.LimitedToStores")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        //comma-separated list of stores used on the list page
        [SeagullResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.LimitedToStores")]
        public string ListOfStores { get; set; }


        public string EncId { get; set; }
        public IList<MessageTemplateLocalizedModel> Locales { get; set; }
    }

    public partial class MessageTemplateLocalizedModel : ILocalizedModelLocal
    {
        public MessageTemplateLocalizedModel()
        {
            AvailableEmailAccounts = new List<SelectListItem>();
        }
        public int LanguageId { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.BccEmailAddresses")]
        [AllowHtml]
        public string BccEmailAddresses { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.EmailAccount")]
        public int EmailAccountId { get; set; }
        public IList<SelectListItem> AvailableEmailAccounts { get; set; }
    }
}