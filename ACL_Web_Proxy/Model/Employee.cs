using System;
using System.ComponentModel;

namespace ACL_Web_Proxy.Model
{
    public class Employee : INotifyPropertyChanged
    {
        private int id;
        private string firstName;
        private string lastName;
        private string principalName;
        private DateTime? expireDate;

        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                if (this.id == value)
                    return;

                this.id = value;
                this.OnPropertyChanged(nameof(this.Id));
            }
        }

        public string FirstName
        {
            get
            {
                return this.firstName;
            }
            set
            {
                if (this.firstName == value)
                    return;

                this.firstName = value;
                this.OnPropertyChanged(nameof(this.FirstName));
            }
        }

        public string LastName
        {
            get
            {
                return this.lastName;
            }
            set
            {
                if (this.lastName == value)
                    return;

                this.lastName = value;
                this.OnPropertyChanged(nameof(this.LastName));
            }
        }

        public string PrincipalName
        {
            get
            {
                return this.principalName;
            }
            set
            {
                if (this.principalName == value)
                    return;

                this.principalName = value;
                this.OnPropertyChanged(nameof(this.PrincipalName));
            }
        }

        public DateTime? ExpireDate
        {
            get
            {
                return this.expireDate;
            }
            set
            {
                if (this.expireDate == value)
                    return;

                this.expireDate = value;
                this.OnPropertyChanged(nameof(this.ExpireDate));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged is null)
                return;

            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public Employee()
        {
        }
    }
}