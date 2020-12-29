namespace ACL_Web_Proxy.Model
{
    public class DefaultValues
    {
        public const string CompanyDomain = "intetics.com.ua";

#if (DEBUG)
        public const string DbServer = "127.0.0.1";
        public const string DbName = "ACL_Web_Proxy_Test";
        public const string AdProxyGroup = "ACL_Web_Proxy_Test";
#else
        public const string DbServer = "RESHETILO2";
        public const string DbName = "ACL_Web_Proxy";
        public const string AdProxyGroup = "ACL_Web_Proxy";
#endif
    }
}
