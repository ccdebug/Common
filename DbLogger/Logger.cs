using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLogger
{
    public class Logger
    {
        public static void Handle(LogMessage logMessage)
        {
            if (logMessage == null) 
                return;

            logMessage.IsHandle = true;

            DisruptorUtils.Publish(logMessage);
        }

        public static void Process(LogMessage logMessage)
        {
            if (logMessage == null)
                return;

            logMessage.IsHandle = false;

            DisruptorUtils.Publish(logMessage);
        }
    }
}
