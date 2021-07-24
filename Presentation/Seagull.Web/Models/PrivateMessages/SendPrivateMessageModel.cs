using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Web.Framework.Mvc;
using Seagull.Web.Validators.PrivateMessages;

namespace Seagull.Web.Models.PrivateMessages
{
    [Validator(typeof(SendPrivateMessageValidator))]
    public partial class SendPrivateMessageModel : BaseSeagullEntityModel
    {
        public int ToUserId { get; set; }
        public string UserToName { get; set; }
        public bool AllowViewingToProfile { get; set; }

        public int ReplyToMessageId { get; set; }

        [AllowHtml]
        public string Subject { get; set; }

        [AllowHtml]
        public string Message { get; set; }
    }
}