
namespace Seagull.Services.Events
{
    public interface IConsumer<T>
    {
        void HandleEvent(T eventMessage);
    }
}
