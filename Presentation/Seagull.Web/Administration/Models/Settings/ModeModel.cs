using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Settings
{
    public partial class ModeModel : BaseSeagullModel
    {
        public string ModeName { get; set; }
        public bool Enabled { get; set; }
    }
}