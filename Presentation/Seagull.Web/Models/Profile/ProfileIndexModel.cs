
namespace Seagull.Web.Models.Profile
{
    public partial class ProfileIndexModel
    {
        public int UserProfileId { get; set; }
        public string ProfileTitle { get; set; }
        public int PostsPage { get; set; }
        public bool PagingPosts { get; set; }
        public bool ForumsEnabled { get; set; }
    }
}