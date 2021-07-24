using System.Collections.Generic;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Messages;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Messages
{
    [Validator(typeof(TestMessageTemplateValidator))]
    public partial class TestMessageTemplateModel : BaseSeagullEntityModel
    {
        public TestMessageTemplateModel()
        {
            Tokens = new List<string>();
        }

        public int LanguageId { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.MessageTemplates.Test.Tokens")]
        public List<string> Tokens { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.MessageTemplates.Test.SendTo")]
        public string SendTo { get; set; }
    }
}