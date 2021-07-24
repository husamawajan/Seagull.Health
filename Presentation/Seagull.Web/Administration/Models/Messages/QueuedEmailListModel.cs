using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Messages
{
    public partial class QueuedEmailListModel : BaseSeagullModel
    {
        [SeagullResourceDisplayName("Admin.System.QueuedEmails.List.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? SearchStartDate { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.List.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? SearchEndDate { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.List.FromEmail")]
        [AllowHtml]
        public string SearchFromEmail { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.List.ToEmail")]
        [AllowHtml]
        public string SearchToEmail { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.List.LoadNotSent")]
        public bool SearchLoadNotSent { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.List.MaxSentTries")]
        public int SearchMaxSentTries { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.List.GoDirectlyToNumber")]
        public int GoDirectlyToNumber { get; set; }
    }
}