using ACL_Web_Proxy.Tools;
using ACL_Web_Proxy.View;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace ACL_Web_Proxy.ViewModel
{
    public partial class MainWindowViewModel : INotifyPropertyChanged
    {
        private OsInfo osInfo;
        private string version;

        public OsInfo OsInfo
        {
            get
            {
                return this.osInfo;
            }
            set
            {
                if (this.osInfo == value)
                    return;

                this.osInfo = value;
                this.OnPropertyChanged(nameof(this.OsInfo));
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged is null)
                return;

            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindowViewModel()
        {
            this.OsInfo = new OsInfo();
            this.Version = Assembly.GetExecutingAssembly().GetName()?.Version.ToString();
        }

        #region Events
        // Код, выполняемый при загрузке окна
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            this.DenySecondRun();
            this.CheckRightAccess();
        }

        // Код, выполняемый при закрытии окна
        private void WindowClosing(object sender, EventArgs e)
        {
            Process[] processList = this.GetProcessList();

            foreach (var process in processList)
            {
                process.Kill();
            }
        }

        // Проверка на запуск 2й копии
        private void DenySecondRun()
        {
            Process[] processList = this.GetProcessList();

            if (processList.Length > 1)
            {
                new Announcer().Show("ПО уже запущено. Данный экземляр будет закрыт.");
                Application.Current.Shutdown();
            }
        }

        // Получение списка процессов
        private Process[] GetProcessList(string p = "")
        {
            if (!string.IsNullOrEmpty(p))
                return Process.GetProcessesByName(p);
            else
                return Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
        }

        // Проверка доступа
        private void CheckRightAccess()
        {
            bool isAllow = false;
            string user = this.OsInfo.CurrentUser;
            string domain = this.OsInfo.CurrentDomain;

            if (user.Contains("_adm") && domain == "INTETICS-UA")
                isAllow = true;

#if (DEBUG)
            isAllow = true;
#endif

            // Проверка доступа на запуск софтины
            if (!isAllow)
            {
                new Announcer().Show("Access is denied.", "Warning");
                this.CloseApplication();
            }
        }

        // Закрытие приложения
        private void CloseApplication()
        {
            Application.Current.Shutdown();
        }
        #endregion
    }
}
