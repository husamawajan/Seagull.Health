using System.Collections.Generic;
using Seagull.Web.Models.Common;

namespace Seagull.Web.Models.Profile
{
    public partial class ProfilePostsModel
    {
        public IList<PostsModel> Posts { get; set; }
    }
}