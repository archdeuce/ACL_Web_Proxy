using ACL_Web_Proxy.Model;
using ACL_Web_Proxy.Tools;
using System;
using System.Reflection;

namespace ACL_Web_Proxy.ViewModel
{
    public class InfoTabViewModel : BaseViewModel
    {
        private string serverName;
        private string domainName;
        private string baseName;
        private string adGroupName;
        private string version;
        private string hint;

        public string ServerName
        {
            get
            {
                return this.serverName;
            }
            set
            {
                if (this.serverName == value)
                    return;

                this.serverName = value;
                this.OnPropertyChanged(nameof(this.ServerName));
            }
        }

        public string DomainName
        {
            get
            {
                return this.domainName;
            }
            set
            {
                if (this.domainName == value)
                    return;

                this.domainName = value;
                this.OnPropertyChanged(nameof(this.DomainName));
            }
        }

        public string BaseName
        {
            get
            {
                return this.baseName;
            }
            set
            {
                if (this.baseName == value)
                    return;

                this.baseName = value;
                this.OnPropertyChanged(nameof(this.BaseName));
            }
        }

        public string AdGroupName
        {
            get
            {
                return this.adGroupName;
            }
            set
            {
                if (this.adGroupName == value)
                    return;

                this.adGroupName = value;
                this.OnPropertyChanged(nameof(this.AdGroupName));
            }
        }

        public string Version
        {
            get
            {
                return this.version;
            }
            set
            {
                if (this.version == value)
                    return;

                this.version = value;
                this.OnPropertyChanged(nameof(this.Version));
            }
        }

        public string Hint
        {
            get
            {
                return this.hint;
            }
            set
            {
                if (this.hint == value)
                    return;

                this.hint = value;
                this.OnPropertyChanged(nameof(this.Hint));
            }
        }

        public InfoTabViewModel()
        {
            this.ServerName = DefaultValues.DbServer;
            this.DomainName = DefaultValues.CompanyDomain;
            this.BaseName = DefaultValues.DbName;
            this.AdGroupName = DefaultValues.AdProxyGroup;
            this.Version = Assembly.GetExecutingAssembly().GetName()?.Version.ToString();
            this.Hint = this.GetBuildDays();
        }

        // Получение информации и количестве дней с момента старта разработки до текущего билда
        private string GetBuildDays()
        {
            string[] buffer = this.Version.Split('.');

            if (buffer.Length > 2)
            {
                int days = int.Parse(buffer[2]);
                string daysExtension;

                switch (days % 10)
                {
                    case 1:
                        daysExtension = "день";
                        break;
                    case 2:
                    case 3:
                    case 4:
                        daysExtension = "дня";
                        break;
                    default:
                        daysExtension = "дней";
                        break;
                }

                return $"Прошло {days} {daysExtension} с момента старта разработки до текущей версии ПО.";
            }

            return string.Empty;
        }
    }
}
