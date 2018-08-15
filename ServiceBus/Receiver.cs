using Chama.WebApi.Models.Utils;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Chama.WebApi.ServiceBus
{
    public interface IReceiver<T>
    {
        void Receive(
            Func<T, MessageProcessResponse> onProcess,
            Action<Exception> onError,
            Action onWait);
    }

    public class Receiver<T> : IReceiver<T> where T : class
    {
        public void Receive(
            Func<T, MessageProcessResponse> onProcess,
            Action<Exception> onError,
            Action onWait)
        {
            var options = new MessageHandlerOptions(e =>
            {
                onError(e.Exception);
                return Task.CompletedTask;
            })
            {
                AutoComplete = false,
                MaxAutoRenewDuration = TimeSpan.FromMinutes(1)
            };
            var connectionString = "Endpoint=sb://chamaqueue.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=q1MKPN84/guik0UAoa4e83W6eg8z6162yFToJiKKuU4=";
            var queueName = "SignUpQueue";

            QueueClient client = new QueueClient(connectionString, queueName, ReceiveMode.ReceiveAndDelete);

            client.RegisterMessageHandler(
                    async (message, token) =>
                    {
                        try
                        {
                        // Get message
                        var data = Encoding.UTF8.GetString(message.Body);
                            T item = JsonConvert.DeserializeObject<T>(data);

                        // Process message
                        var result = onProcess(item);

                            if (result == MessageProcessResponse.Complete)
                                await client.CompleteAsync(message.SystemProperties.LockToken);
                            else if (result == MessageProcessResponse.Abandon)
                                await client.AbandonAsync(message.SystemProperties.LockToken);
                            else if (result == MessageProcessResponse.Dead)
                                await client.DeadLetterAsync(message.SystemProperties.LockToken);

                        // Wait for next message
                        onWait();
                        }
                        catch (Exception ex)
                        {
                            await client.DeadLetterAsync(message.SystemProperties.LockToken);
                            onError(ex);
                        }
                    }, options);
        }

    }
}
