using Microsoft.Win32;
using System;

namespace ACL_Web_Proxy.Model
{
    public class Settings
    {
        private string SoftWareName { get; set; }
        public int SelectedLines { get; set;  }

        public Settings()
        {
            this.SoftWareName = "ACL_Web_Proxy";
            this.SelectedLines = 25;

            this.Read();
        }

        private void Read()
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(this.SoftWareName);

            if (regKey == null)
            {
                this.Save();
                return;
            }

            string regValueSelectedLines = regKey.GetValue(nameof(this.SelectedLines), "NULL").ToString();

            if (regValueSelectedLines != "NULL")
                this.SelectedLines = Convert.ToInt32(regValueSelectedLines);

            regKey.Close();
        }

        public void Save()
        {
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey(this.SoftWareName);
            regKey.SetValue(nameof(this.SelectedLines), this.SelectedLines.ToString());
            regKey.Close();
        }
    }
}
