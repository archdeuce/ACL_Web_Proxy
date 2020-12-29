using ACL_Web_Proxy.Services;

namespace ACL_Web_Proxy.ViewModel
{
    public class ViewModelLocator
    {
        private readonly IEmployeeService employeeService;

        public MainWindowViewModel MainWindowViewModel
        {
            get
            {
                return new MainWindowViewModel();
            }
        }

        public MainTabViewModel MainTabViewModel
        {
            get
            {
                return new MainTabViewModel(this.employeeService);
            }
        }

        public LogTabViewModel LogTabViewModel
        {
            get
            {
                return new LogTabViewModel(this.employeeService);
            }
        }

        public InfoTabViewModel InfoTabViewModel
        {
            get
            {
                return new InfoTabViewModel();
            }
        }

        public ViewModelLocator()
        {
            this.employeeService = new EmployeeService();
        }
    }
}