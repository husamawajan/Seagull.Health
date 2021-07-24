using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Users;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Seagull.Admin.Models.Users
{
    [Validator(typeof(UserRoleValidator))]
    public partial class UserRoleModel : BaseSeagullEntityModel
    {
        public int Id { get; set; }
        [SeagullResourceDisplayName("Admin.Users.UserRoles.Fields.Name")]
        [AllowHtml]
        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
        public string Name { get; set; }

        [SeagullResourceDisplayName("Admin.Users.UserRoles.Fields.Active")]
        public bool Active { get; set; }

        [SeagullResourceDisplayName("Admin.Users.UserRoles.Fields.IsSystemRole")]
        public bool IsSystemRole { get; set; }

        [SeagullResourceDisplayName("Admin.Users.UserRoles.Fields.SystemName")]
        //[Required]
        [StringLength(50)]
        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
        public string SystemName { get; set; }

        [SeagullResourceDisplayName("Admin.Users.UserRoles.Fields.EnablePasswordLifetime")]
        public bool EnablePasswordLifetime { get; set; }

        [SeagullResourceDisplayName("Admin.Users.UserRoles.Fields.PurchasedWithProduct")]
        public int PurchasedWithProductId { get; set; }

        [SeagullResourceDisplayName("Admin.Users.UserRoles.Fields.PurchasedWithProduct")]
        public string PurchasedWithProductName { get; set; }
        public string EncId { get; set; } // Encrypt Url

        #region Nested classes

        public partial class AssociateProductToUserRoleModel : BaseSeagullModel
        {
            public AssociateProductToUserRoleModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableProductTypes = new List<SelectListItem>();
            }

            [SeagullResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
            [AllowHtml]
            public string SearchProductName { get; set; }
            [SeagullResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [SeagullResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }
            [SeagullResourceDisplayName("Admin.Catalog.Products.List.SearchStore")]
            public int SearchStoreId { get; set; }

            [SeagullResourceDisplayName("Admin.Catalog.Products.List.SearchProductType")]
            public int SearchProductTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableProductTypes { get; set; }




            public int AssociatedToProductId { get; set; }
        }
        #endregion
    }
}