using System;
using NLog;

namespace Trader
{
    internal class LogUtils
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        internal static void Debug(string v)
        {
            logger.Debug(v);
        }
    }
}