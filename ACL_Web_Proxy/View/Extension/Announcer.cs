using System.Windows;

namespace ACL_Web_Proxy.View
{
    public class Announcer
    {
        public MessageBoxResult Show(string message, string title = "Warning")
        {
            try
            {
                if (title.Contains("Warning"))
                    return MessageBoxEx.Show(Application.Current.MainWindow, message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
                else if (title.Contains("Information"))
                    return MessageBoxEx.Show(Application.Current.MainWindow, message, title, MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    return MessageBoxEx.Show(Application.Current.MainWindow, message, title, MessageBoxButton.OK);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public MessageBoxResult Show(System.Exception e, string msg = "Error", string status = "Exception")
        {
            return MessageBoxEx.Show(Application.Current.MainWindow, msg, e.Message.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public MessageBoxResult Question(string message, string title = "")
        {
            return MessageBoxEx.Show(Application.Current.MainWindow, message, title, MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);
        }
    }
}
