using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;
using Seagull.Web.Validators.Common;

namespace Seagull.Web.Models.Common
{
    [Validator(typeof(ContactUsValidator))]
    public partial class ContactUsModel : BaseSeagullModel
    {
        [AllowHtml]
        [SeagullResourceDisplayName("ContactUs.Email")]
        public string Email { get; set; }

        [AllowHtml]
        [SeagullResourceDisplayName("ContactUs.Subject")]
        public string Subject { get; set; }
        public bool SubjectEnabled { get; set; }

        [AllowHtml]
        [SeagullResourceDisplayName("ContactUs.Enquiry")]
        public string Enquiry { get; set; }

        [AllowHtml]
        [SeagullResourceDisplayName("ContactUs.FullName")]
        public string FullName { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}