using System.Collections.Generic;
using Seagull.Web.Models.Common;

namespace Seagull.Web.Models.PrivateMessages
{
    public partial class PrivateMessageListModel
    {
        public IList<PrivateMessageModel> Messages { get; set; }
    }
}