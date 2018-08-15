using Chama.WebApi.Log;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chama.WebApi.ServiceBus
{
    public interface ISender<T>
    {
        Task SendAsync(T item);
        Task SendAsync(T item, Dictionary<string, object> properties);
    }

    public class Sender<T> : ISender<T> where T : class
    {
        private QueueClient client;        

        public async Task SendAsync(T item)
        {
            await SendAsync(item, null);
        }

        public async Task SendAsync(T item, Dictionary<string, object> properties)
        {
            try
            {
                var connectionString = "Endpoint=sb://chamaqueue.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=q1MKPN84/guik0UAoa4e83W6eg8z6162yFToJiKKuU4=";

                var queueName = "SignUpQueue";

                client = new QueueClient(connectionString, queueName, ReceiveMode.ReceiveAndDelete);

                var json = JsonConvert.SerializeObject(item);
                var message = new Message(Encoding.UTF8.GetBytes(json));

                if (properties != null)
                {
                    foreach (var prop in properties)
                    {
                        message.UserProperties.Add(prop.Key, prop.Value);
                    }
                }

                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

    }
}
