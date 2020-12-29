using System;

namespace ACL_Web_Proxy.Model
{
    public class Log
    {
        public int Id { get; set; }
        public string Executor { get; set; }
        public string Employee { get; set; }
        public string Operation { get; set; }
        public string Host { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime? ExpireDate { get; set; }

        public Log()
        {
            this.EventDate = DateTime.Now;
        }
    }
}
