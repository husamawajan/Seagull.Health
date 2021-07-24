using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Messages;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Messages
{
    [Validator(typeof(QueuedEmailValidator))]
    public partial class QueuedEmailModel: BaseSeagullEntityModel
    {
        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.Id")]
        public override int Id { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.Priority")]
        public string PriorityName { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.From")]
        [AllowHtml]
        public string From { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.FromName")]
        [AllowHtml]
        public string FromName { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.To")]
        [AllowHtml]
        public string To { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.ToName")]
        [AllowHtml]
        public string ToName { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.ReplyTo")]
        [AllowHtml]
        public string ReplyTo { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.ReplyToName")]
        [AllowHtml]
        public string ReplyToName { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.CC")]
        [AllowHtml]
        public string CC { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.Bcc")]
        [AllowHtml]
        public string Bcc { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.AttachmentFilePath")]
        [AllowHtml]
        public string AttachmentFilePath { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.AttachedDownload")]
        [UIHint("Download")]
        public int AttachedDownloadId { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.SendImmediately")]
        public bool SendImmediately { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.DontSendBeforeDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? DontSendBeforeDate { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.SentTries")]
        public int SentTries { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.SentOn")]
        public DateTime? SentOn { get; set; }

        [SeagullResourceDisplayName("Admin.System.QueuedEmails.Fields.EmailAccountName")]
        [AllowHtml]
        public string EmailAccountName { get; set; }
    }
}