using ACL_Web_Proxy.Model;
using ACL_Web_Proxy.Services;
using ACL_Web_Proxy.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ACL_Web_Proxy.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        internal IEmployeeService employeeService;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged is null)
                return;

            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
