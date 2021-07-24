using Seagull.Web.Framework.Mvc;

namespace Seagull.Web.Models.Common
{
    public partial class LanguageModel : BaseSeagullEntityModel
    {
        public string Name { get; set; }

        public string FlagImageFileName { get; set; }

    }
}