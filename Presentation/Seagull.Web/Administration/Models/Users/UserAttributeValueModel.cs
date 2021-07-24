using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Users;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Localization;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Users
{
    [Validator(typeof(UserAttributeValueValidator))]
    public partial class UserAttributeValueModel : BaseSeagullEntityModel, ILocalizedModel<UserAttributeValueLocalizedModel>
    {
        public UserAttributeValueModel()
        {
            Locales = new List<UserAttributeValueLocalizedModel>();
        }

        public int UserAttributeId { get; set; }

        [SeagullResourceDisplayName("Admin.Users.UserAttributes.Values.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SeagullResourceDisplayName("Admin.Users.UserAttributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [SeagullResourceDisplayName("Admin.Users.UserAttributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder {get;set;}

        public IList<UserAttributeValueLocalizedModel> Locales { get; set; }

    }

    public partial class UserAttributeValueLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [SeagullResourceDisplayName("Admin.Users.UserAttributes.Values.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}