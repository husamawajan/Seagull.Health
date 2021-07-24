using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Messages;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Messages
{
    [Validator(typeof(CampaignValidator))]
    public partial class CampaignModel : BaseSeagullEntityModel
    {
        public CampaignModel()
        {
            this.AvailableStores = new List<SelectListItem>();
            this.AvailableUserRoles = new List<SelectListItem>();
            this.AvailableEmailAccounts = new List<SelectListItem>();
        }

        [SeagullResourceDisplayName("Admin.Promotions.Campaigns.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SeagullResourceDisplayName("Admin.Promotions.Campaigns.Fields.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [SeagullResourceDisplayName("Admin.Promotions.Campaigns.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [SeagullResourceDisplayName("Admin.Promotions.Campaigns.Fields.Store")]
        public int StoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        [SeagullResourceDisplayName("Admin.Promotions.Campaigns.Fields.UserRole")]
        public int UserRoleId { get; set; }
        public IList<SelectListItem> AvailableUserRoles { get; set; }

        [SeagullResourceDisplayName("Admin.Promotions.Campaigns.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [SeagullResourceDisplayName("Admin.Promotions.Campaigns.Fields.DontSendBeforeDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? DontSendBeforeDate { get; set; }

        [SeagullResourceDisplayName("Admin.Promotions.Campaigns.Fields.AllowedTokens")]
        public string AllowedTokens { get; set; }

        [SeagullResourceDisplayName("Admin.Promotions.Campaigns.Fields.EmailAccount")]
        public int EmailAccountId { get; set; }
        public IList<SelectListItem> AvailableEmailAccounts { get; set; }

        [SeagullResourceDisplayName("Admin.Promotions.Campaigns.Fields.TestEmail")]
        [AllowHtml]
        public string TestEmail { get; set; }
    }
}