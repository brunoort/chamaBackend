using NLog.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chama.WebApi.Log
{    
    public static class Logger
    {
        public static void Log(string message)
        {
            var logger = new LoggerAdapter(NLog.LogManager.GetCurrentClassLogger());
            logger.Info(message);
        }

        public static void Log(Exception exception)
        {
            var logger = new LoggerAdapter(NLog.LogManager.GetCurrentClassLogger());
            logger.ErrorException(exception.Message, exception);
        }
    }
}
