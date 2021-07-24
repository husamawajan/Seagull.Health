using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Users
{
    public partial class UserListModel : BaseSeagullModel
    {
        public UserListModel()
        {
            SearchUserRoleIds = new List<int>();
            AvailableUserRoles = new List<SelectListItem>();
        }

        [UIHint("MultiSelect")]
        [SeagullResourceDisplayName("Admin.Users.Users.List.UserRoles")]
        public IList<int> SearchUserRoleIds { get; set; }
        public IList<SelectListItem> AvailableUserRoles { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Users.List.SearchEmail")]
        [AllowHtml]
        public string SearchEmail { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Users.List.SearchUsername")]
        [AllowHtml]
        public string SearchUsername { get; set; }
        public bool UsernamesEnabled { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Users.List.SearchFirstName")]
        [AllowHtml]
        public string SearchFirstName { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Users.List.SearchLastName")]
        [AllowHtml]
        public string SearchLastName { get; set; }


        [SeagullResourceDisplayName("Admin.Users.Users.List.SearchDateOfBirth")]
        [AllowHtml]
        public string SearchDayOfBirth { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Users.List.SearchDateOfBirth")]
        [AllowHtml]
        public string SearchMonthOfBirth { get; set; }
        public bool DateOfBirthEnabled { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Users.List.SearchPhone")]
        [AllowHtml]
        public string SearchPhone { get; set; }
        public bool PhoneEnabled { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Users.List.SearchZipCode")]
        [AllowHtml]
        public string SearchZipPostalCode { get; set; }
        public bool ZipPostalCodeEnabled { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Users.List.SearchIpAddress")]
        public string SearchIpAddress { get; set; }
    }
}