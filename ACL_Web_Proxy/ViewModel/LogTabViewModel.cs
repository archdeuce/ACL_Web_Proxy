using ACL_Web_Proxy.Model;
using ACL_Web_Proxy.Services;
using ACL_Web_Proxy.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ACL_Web_Proxy.ViewModel
{
    public class LogTabViewModel : BaseViewModel
    {
        private string filter;
        private int selectedLines;
        private readonly Settings settings;
        private ObservableCollection<Log> dbLogs;
        public ObservableCollection<int> LineNumbers { get; set; }

        public int SelectedLines
        {
            get
            {
                return this.selectedLines;
            }
            set
            {
                if (this.selectedLines == value)
                    return;

                this.selectedLines = value;
                this.OnPropertyChanged(nameof(this.SelectedLines));
            }
        }
        public string Filter
        {
            get
            {
                return this.filter;
            }
            set
            {
                if (this.filter == value)
                    return;

                this.filter = value;
                this.OnPropertyChanged(nameof(this.Filter));
            }
        }

        public ObservableCollection<Log> DbLogs
        {
            get
            {
                return this.dbLogs;
            }
            set
            {
                if (this.dbLogs == value)
                    return;

                this.dbLogs = value;
                this.OnPropertyChanged(nameof(this.DbLogs));
            }
        }

        public ICommand SearchCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
        public ICommand SearchTextBoxCommand { get; set; }

        public LogTabViewModel(IEmployeeService employeeService)
        {
            this.settings = new Settings();
            this.employeeService = employeeService;
            this.DbLogs = new ObservableCollection<Log>();
            this.LineNumbers = new ObservableCollection<int>() { 25, 50, 100, 250, 500, 1000, 10000 };
            this.SelectedLines = this.settings.SelectedLines;

            this.SearchCommand = new RelayCommand(this.SearchCommandExecuted, this.SearchCommandCanExecute);
            this.LoadedCommand = new RelayCommand(this.LoadedCommandExecuted, this.LoadedCommandCanExecute);
            this.SearchTextBoxCommand = new RelayCommand(this.SearchTextBoxCommandExecuted, this.SearchTextBoxCommandCanExecute);
        }

        #region Methods
        // Получение логов из базы
        private void GetLogs(string filter, int takeLines = 25, int skipLines = 0)
        {
            this.SaveSelectedLines(takeLines);

            var logs = this.employeeService.GetLogs().ToList().OrderByDescending(l => l.EventDate);

            if (string.IsNullOrEmpty(filter))
            {
                this.DbLogs = new ObservableCollection<Log>(logs.Take(takeLines));
            }
            else
            {
                string f = filter.ToLower();

                this.DbLogs = new ObservableCollection<Log>(logs
                    .Where(x => x.Executor.ToLower().Contains(f) ||
                                x.Employee.ToLower().Contains(f) ||
                                x.EventDate.ToString().Contains(f) ||
                                x.Operation.ToLower().Contains(f))
                    .Take(takeLines));
            }
        }

        // Очистка поля поиска с небольшой задержкой
        private void ClearSearchField()
        {
            this.Filter = string.Empty;
            this.SearchCommandExecuted(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        // Принудительная установка раскладки клавиатуры
        private void SetKeyboardLayout(string layout = "en-US")
        {
            System.Windows.Forms.InputLanguage.CurrentInputLanguage = System.Windows.Forms.InputLanguage.FromCulture(new System.Globalization.CultureInfo(layout));
        }

        // Сохранение в реестр настроек фильтра
        private void SaveSelectedLines(int selectedLines)
        {
            this.settings.SelectedLines = selectedLines;
            this.settings.Save();
        }
        #endregion

        #region Commands
        // Код, выполняемый при загрузке UserControl-a
        private void LoadedCommandExecuted(object obj)
        {
            this.SetKeyboardLayout("en-US");
            this.SearchCommandExecuted(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private bool LoadedCommandCanExecute(object obj)
        {
            return true;
        }

        // Обработчик кнопки фильтрации
        private void SearchCommandExecuted(object obj)
        {
            Task.Delay(500)
                .ContinueWith((task) =>
                {
                    this.GetLogs(this.Filter, this.SelectedLines);
                });
        }

        private bool SearchCommandCanExecute(object obj)
        {
            return true;
        }

        // Взаимодействие с полем ввода
        private void SearchTextBoxCommandExecuted(object obj)
        {
            this.ClearSearchField();
        }

        private bool SearchTextBoxCommandCanExecute(object obj)
        {
            return true;
        }
        #endregion
    }
}
