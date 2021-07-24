using Seagull.Core;
using Seagull.Core.Domain.Messages;
using Seagull.Services.Events;

namespace Seagull.Services.Messages
{
    public static class EventPublisherExtensions
    {
       

       
        public static void EntityTokensAdded<T, U>(this IEventPublisher eventPublisher, T entity, System.Collections.Generic.IList<U> tokens) where T : BaseEntity
        {
            eventPublisher.Publish(new EntityTokensAddedEvent<T, U>(entity, tokens));
        }

        public static void MessageTokensAdded<U>(this IEventPublisher eventPublisher, MessageTemplate message, System.Collections.Generic.IList<U> tokens)
        {
            eventPublisher.Publish(new MessageTokensAddedEvent<U>(message, tokens));
        }
    }
}