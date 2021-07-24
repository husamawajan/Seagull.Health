using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Users;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Localization;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Users
{
    [Validator(typeof(UserAttributeValidator))]
    public partial class UserAttributeModel : BaseSeagullEntityModel, ILocalizedModel<UserAttributeLocalizedModel>
    {
        public UserAttributeModel()
        {
            Locales = new List<UserAttributeLocalizedModel>();
        }

        [SeagullResourceDisplayName("Admin.Users.UserAttributes.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SeagullResourceDisplayName("Admin.Users.UserAttributes.Fields.IsRequired")]
        public bool IsRequired { get; set; }

        [SeagullResourceDisplayName("Admin.Users.UserAttributes.Fields.AttributeControlType")]
        public int AttributeControlTypeId { get; set; }
        [SeagullResourceDisplayName("Admin.Users.UserAttributes.Fields.AttributeControlType")]
        [AllowHtml]
        public string AttributeControlTypeName { get; set; }

        [SeagullResourceDisplayName("Admin.Users.UserAttributes.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }


        public IList<UserAttributeLocalizedModel> Locales { get; set; }

    }

    public partial class UserAttributeLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [SeagullResourceDisplayName("Admin.Users.UserAttributes.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

    }
}