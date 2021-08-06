using Leaf.xNet.Services.Cloudflare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TenMinuteMailCom
{
    public class TenMinuteMailCom
    {
        private Leaf.xNet.HttpRequest http;
        private string email = "";
        private string token = "";
        public TenMinuteMailCom(string token = "")
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3 | System.Net.SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.DefaultConnectionLimit = 256;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.UseNagleAlgorithm = true;
            http = new Leaf.xNet.HttpRequest();
            http.IgnoreProtocolErrors = true;
            http.SslProtocols = System.Security.Authentication.SslProtocols.Tls | System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls11;
            if (!string.IsNullOrEmpty(token))
            {
                LoginEmail(token);
            }
            else
            {
                http.GetThroughCloudflare("https://web2.10minemail.com/mailbox");
                string post = http.Post("https://web2.10minemail.com/mailbox").ToString();
                this.email = Regex.Match(post, "\"mailbox\":\"(.*?)\"").Groups[1].Value;
                this.token = Regex.Match(post, "\"token\":\"(.*?)\"").Groups[1].Value;
            }
        }
        public string GetEmail()
        {
            http.GetThroughCloudflare("https://web2.10minemail.com/messages");
            http.AddHeader("Authorization", "Bearer " + token);
            string get = http.Get("https://web2.10minemail.com/messages").ToString();
            return Regex.Match(get, "\"mailbox\":\"(.*?)\"").Groups[1].Value;
        }
        public string NewEmail()
        {
            http.GetThroughCloudflare("https://web2.10minemail.com/mailbox");
            string post = http.Post("https://web2.10minemail.com/mailbox").ToString();
            email = Regex.Match(post, "\"mailbox\":\"(.*?)\"").Groups[1].Value;
            token = Regex.Match(post, "\"token\":\"(.*?)\"").Groups[1].Value;
            return email;
        }
        public string GetToken()
        {
            return token;
        }
        public string LoginEmail(string token)
        {
            http.GetThroughCloudflare("https://web2.10minemail.com/messages");
            http.AddHeader("Authorization", "Bearer " + token);
            string get = http.Get("https://web2.10minemail.com/messages").ToString();
            email = Regex.Match(get, "\"mailbox\":\"(.*?)\"").Groups[1].Value;
            this.token = Regex.Match(get, "\"token\":\"(.*?)\"").Groups[1].Value;
            return email;
        }
        public List<string> GetMailId()
        {
            List<string> list = new List<string>();
            http.GetThroughCloudflare("https://web2.10minemail.com/messages");
            http.AddHeader("Authorization", "Bearer " + token);
            string get = http.Get("https://web2.10minemail.com/messages").ToString();
            string message = Regex.Match(get, "\"messages\":\\[(.*?)]}").Groups[1].Value;
            foreach (var item in Regex.Split(message, "},"))
            {
                string _id = Regex.Match(item, "\"_id\":\"(.*?)\"").Groups[1].Value;
                string receivedAt = Regex.Match(item, "\"receivedAt\":(.*?),").Groups[1].Value;
                string from = Regex.Match(item, "\"from\":\"(.*?)\"").Groups[1].Value;
                string subject = Regex.Match(item, "\"subject\":\"(.*?)\"").Groups[1].Value;
                string bodyPreview = Regex.Match(item, "\"bodyPreview\":\"(.*?)\"").Groups[1].Value;
                string attachmentsCount = Regex.Match(item, "\"attachmentsCount\":(.*?)}").Groups[1].Value;
                list.Add("{\"_id\":\"" + _id + "\",\"receivedAt\":" + receivedAt + ",\"from\":\"" + from + "\",\"subject\":\"" + subject + "\",\"bodyPreview\":\"" + bodyPreview + "\",\"attachmentsCount\":" + attachmentsCount + "}");
            }
            return list;
        }
        public string ReadMail(string id)
        {
            http.GetThroughCloudflare("https://web2.10minemail.com/messages/" + id);
            http.AddHeader("Authorization", "Bearer " + token);
            return http.Get("https://web2.10minemail.com/messages/" + id).ToString();
        }
    }
    
}
