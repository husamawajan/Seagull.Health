using System.Collections.Generic;
using Seagull.Core.Domain.Users;
using Seagull.Core.Domain.Messages;
using Seagull.Core.Domain.Stores;

namespace Seagull.Services.Messages
{
    public partial interface IMessageTokenProvider
    {
        /// <summary>
        /// Add store tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="store">Store</param>
        /// <param name="emailAccount">Email account</param>
        void AddStoreTokens(IList<Token> tokens, Store store, EmailAccount emailAccount);



        /// <summary>
        /// Add user tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="user">User</param>
        void AddUserTokens(IList<Token> tokens, User user);


        /// <summary>
        /// Get collection of allowed (supported) message tokens
        /// </summary>
        /// <param name="tokenGroups">Collection of token groups; pass null to get all available tokens</param>
        /// <returns>Collection of allowed message tokens</returns>
        IEnumerable<string> GetListOfAllowedTokens(IEnumerable<string> tokenGroups = null);
    }
}
