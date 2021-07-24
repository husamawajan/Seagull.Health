using Seagull.Core.Domain.Users;
using Seagull.Web.Models.PrivateMessages;

namespace Seagull.Web.Factories
{
    /// <summary>
    /// Represents the interface of the private message model factory
    /// </summary>
    public partial interface IPrivateMessagesModelFactory
    {
        /// <summary>
        /// Prepare the private message index model
        /// </summary>
        /// <param name="page">Number of items page; pass null to disable paging</param>
        /// <param name="tab">Tab name</param>
        /// <returns>Private message index model</returns>
        PrivateMessageIndexModel PreparePrivateMessageIndexModel(int? page, string tab);


    }
}
