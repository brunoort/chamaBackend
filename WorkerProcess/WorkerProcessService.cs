using Chama.WebApi.Log;
using System;
using System.Threading;
using System.Threading.Tasks;
using Chama.WebApi.ServiceBus;
using Chama.WebApi.ModelView;
using Microsoft.AspNetCore.Mvc;
using Chama.WebApi.Models.Utils;
using Chama.WebApi.Controllers;

namespace Chama.WebApi.WorkerProcess
{
    public class WorkerProcessService : BackgroundService
    {
        private readonly ICoursesController _controller;
        public WorkerProcessService(ICoursesController controller)
        {
            _controller = controller;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.Log("WorkerProcessService is starting.");

            stoppingToken.Register(() =>
                    Logger.Log("WorkerProcessService task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Logger.Log("WorkerProcessService task doing background work.");

                    IReceiver<SignUpModelView> receiver = new Receiver<SignUpModelView>();
                    receiver.Receive(
                        message =>
                        {
                            var result = _controller.SignUp(message) as JsonResult;
                            if (result.Value != null && result.Value is RequestResult && (result.Value as RequestResult).State == RequestState.Success)
                            {
                                //send email
                            }

                            return MessageProcessResponse.Complete;
                        },
                        ex => Logger.Log(ex.Message),
                        () => Logger.Log("Receiving Queue Message..."));

                    await Task.Delay(5000, stoppingToken);
                }

                catch(Exception ex)
                {
                    Logger.Log(ex);
                }
            }

            Logger.Log("WorkerProcessService task is stopping.");
        }

    }
}
