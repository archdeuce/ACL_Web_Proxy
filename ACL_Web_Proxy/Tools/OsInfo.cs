using System;

namespace ACL_Web_Proxy.Tools
{
    public class OsInfo
    {
        public string CurrentUser { get; private set; }
        public string CurrentDomain { get; private set; }
        public string MachineName { get; private set; }
        public string CurrentDirectory { get; private set; }

        public OsInfo()
        {
            this.CurrentUser = Environment.UserName;
            this.CurrentDomain = Environment.UserDomainName;
            this.MachineName = Environment.MachineName;
            this.CurrentDirectory = Environment.CurrentDirectory;
        }
    }
}