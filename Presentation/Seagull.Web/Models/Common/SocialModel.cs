using Seagull.Web.Framework.Mvc;

namespace Seagull.Web.Models.Common
{
    public partial class SocialModel : BaseSeagullModel
    {
        public string FacebookLink { get; set; }
        public string TwitterLink { get; set; }
        public string YoutubeLink { get; set; }
        public string GooglePlusLink { get; set; }
        public int WorkingLanguageId { get; set; }
        public bool NewsEnabled { get; set; }
    }
}