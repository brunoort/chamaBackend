using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chama.WebApi.WorkerProcess
{
    public interface IHostedWorkerProcessService
    { 
        Task StartAsyncProcess(CancellationToken cancellationToken);
        Task StopAsyncProcess(CancellationToken cancellationToken);

    }
}
