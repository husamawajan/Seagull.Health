using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Seagull.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull.Admin
{
    public class SignalRMethods : Hub
    {
        [HubMethodName("sendPriorityNotifyUser")]
        public static void SendPriorityNotifyUser(List<CustomNotifyMsg> data)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SignalRMethods>();
            context.Clients.All.sendPriorityNotifyUser(data);
        }
    }
}