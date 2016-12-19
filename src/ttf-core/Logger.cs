using System;
using NLog;

namespace Ttf.Server.Core
{
    public class Logger
    {
        private static readonly NLog.Logger InternalLogger = LogManager.GetCurrentClassLogger();

        public static void Warn(string message)
        {
            InternalLogger.Warn(message);
        }

        public static void Error(string message, Exception exception = null)
        {
            if (exception == null)
                InternalLogger.Error(message);
            else
                InternalLogger.Error(exception, message);
        }

        public static void Debug(string message)
        {
            InternalLogger.Debug(message);
        }

        public static void Info(string message)
        {
            InternalLogger.Info(message);
        }
    }
}