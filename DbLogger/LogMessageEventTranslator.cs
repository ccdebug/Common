using Disruptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLogger
{
    public class LogMessageEventTranslator : IEventTranslatorOneArg<LogMessageEvent, LogMessage>
    {
        public void TranslateTo(LogMessageEvent @event, long sequence, LogMessage logMessage)
        {
            @event.LogMessage = logMessage;
        }
    }
}
