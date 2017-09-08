using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLogger
{
    public class LogMessage
    {
        public int Id { get; set; }

        public string Ikey { get; set; }

        public string Username { get; set; }

        public DateTime LogTime { get; set; }

        public string ClientIp { get; set; }

        public string ServerIp { get; set; }

        public string Module { get; set; }

        public string OrderNo { get; set; }

        public string LogType { get; set; }

        public string Content { get; set; }

        public string Keyword { get; set; }

        public bool IsHandle { get; set; }
    }
}
