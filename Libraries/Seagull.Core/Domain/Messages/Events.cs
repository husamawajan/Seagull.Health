using System.Collections.Generic;

namespace Seagull.Core.Domain.Messages
{
    public class EmailSubscribedEvent
    {

       
    }

    public class EmailUnsubscribedEvent
    {


        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(EmailUnsubscribedEvent)) return false;
            return Equals((EmailUnsubscribedEvent)obj);
        }

       
    }

    /// <summary>
    /// A container for tokens that are added.
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="U"></typeparam>
    public class EntityTokensAddedEvent<T, U> where T : BaseEntity
    {
        private readonly T _entity;
        private readonly IList<U> _tokens;

        public EntityTokensAddedEvent(T entity, IList<U> tokens)
        {
            _entity = entity;
            _tokens = tokens;
        }

        public T Entity { get { return _entity; } }
        public IList<U> Tokens { get { return _tokens; } }
    }

    /// <summary>
    /// A container for tokens that are added.
    /// </summary>
    /// <typeparam name="U"></typeparam>
    public class MessageTokensAddedEvent<U>
    {
        private readonly MessageTemplate _message;
        private readonly IList<U> _tokens;

        public MessageTokensAddedEvent(MessageTemplate message, IList<U> tokens)
        {
            _message = message;
            _tokens = tokens;
        }

        public MessageTemplate Message { get { return _message; } }
        public IList<U> Tokens { get { return _tokens; } }
    }

    public class AdditionTokensAddedEvent
    {
        private readonly IList<string> _tokens;

        public AdditionTokensAddedEvent()
        {
            this._tokens=new List<string>();
        }

        public void AddTokens(params string[] additionTokens)
        {
            foreach (var additionToken in additionTokens)
            {
                this._tokens.Add(additionToken);
            }
        }

        public IList<string> AdditionTokens { get { return _tokens; } }
    }

    public class CampaignAdditionTokensAddedEvent : AdditionTokensAddedEvent
    {
    }
}