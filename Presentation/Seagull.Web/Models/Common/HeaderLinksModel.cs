using Seagull.Web.Framework.Mvc;

namespace Seagull.Web.Models.Common
{
    public partial class HeaderLinksModel : BaseSeagullModel
    {
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        
        public bool ShoppingCartEnabled { get; set; }
        public int ShoppingCartItems { get; set; }
        
        public bool WishlistEnabled { get; set; }
        public int WishlistItems { get; set; }

        public bool AllowPrivateMessages { get; set; }
        public string UnreadPrivateMessages { get; set; }
        public string AlertMessage { get; set; }
    }
}