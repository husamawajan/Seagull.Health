
using Owin;
using Microsoft.Owin;
[assembly: OwinStartup(typeof(SignalRChat.SignalRStartup))]
namespace SignalRChat
{
    public class SignalRStartup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}