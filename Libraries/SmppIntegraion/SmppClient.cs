using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SmppIntegraion
{
    public class SmppClient
    {
        private string loginName;
        private string password;
        private string senderName;
        private string messageBody;
        private string messageRecipients;
        private int tracking;
        private int mobType;

        public SmppClient(string loginName, string password, string senderName,
            int tracking = 1, int mobType = 1)
        {
            this.loginName = loginName;
            this.password = password;
            this.senderName = senderName;
            this.tracking = tracking;
            this.mobType = mobType;
        }

        public bool SendSMS(string messageRecipients, string messageBody)
        {
            try
            {
                if (!String.IsNullOrEmpty(messageRecipients) && !String.IsNullOrEmpty(messageBody))
                {
                    //call Web Service 
                    this.messageRecipients = messageRecipients;
                    this.messageBody = messageBody;
                    string url = prepareMessage();
                    if (!String.IsNullOrEmpty(url))
                    {
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                               | SecurityProtocolType.Tls11
                               | SecurityProtocolType.Tls12
                               | SecurityProtocolType.Ssl3;
                        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                        request.Method = "GET";
                        String test = String.Empty;
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), ASCIIEncoding.ASCII))
                                {
                                    string responseText = reader.ReadToEnd();
                                }
                                return true;
                            }
                        }
                    }
                }
                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string prepareMessage()
        {
            try
            {
                //string url = "http://http.redrabbitsms.com/epicenter/gatewaysendG.asp?" +
                string url = "https://app.redrabbitsms.com/epicenter/gatewaysend.asp?" +
                    "LoginName=" + this.loginName +
                    "&Password=" + this.password +
                    "&Tracking=" + this.tracking +
                    "&Mobtyp=" + this.mobType +
                    "&MsgTyp=" + 5 +
                    "&MessageRecipients=" + this.messageRecipients +
                    "&MessageBody=" + this.messageBody +
                    "&SenderName=" + this.senderName;

                return url;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}