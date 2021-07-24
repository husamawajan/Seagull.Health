using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Common
{
    public partial class SystemWarningModel : BaseSeagullModel
    {
        public SystemWarningLevel Level { get; set; }

        public string Text { get; set; }
    }

    public enum SystemWarningLevel
    {
        Pass,
        Warning,
        Fail
    }
}