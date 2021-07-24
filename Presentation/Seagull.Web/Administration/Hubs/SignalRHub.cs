using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Seagull.Admin.Hubs
{
    public class SignalRHub : Hub
    {
        public void Send(string name , string message)
        {
            Clients.All.SendMessage(name, message);
        }
        public static void SendPriorityNotifyUser(int NumberOfNotify)
        {
            //Clients.All.SendPriorityNotifyUser(NumberOfNotify);
        }
    }
}