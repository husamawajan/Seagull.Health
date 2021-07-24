using System;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Users
{
    public partial class OnlineUserModel : BaseSeagullEntityModel
    {
        [SeagullResourceDisplayName("Admin.Users.OnlineUsers.Fields.UserInfo")]
        public string UserInfo { get; set; }

        [SeagullResourceDisplayName("Admin.Users.OnlineUsers.Fields.IPAddress")]
        public string LastIpAddress { get; set; }

        [SeagullResourceDisplayName("Admin.Users.OnlineUsers.Fields.Location")]
        public string Location { get; set; }

        [SeagullResourceDisplayName("Admin.Users.OnlineUsers.Fields.LastActivityDate")]
        public DateTime LastActivityDate { get; set; }
        
        [SeagullResourceDisplayName("Admin.Users.OnlineUsers.Fields.LastVisitedPage")]
        public string LastVisitedPage { get; set; }
    }
}