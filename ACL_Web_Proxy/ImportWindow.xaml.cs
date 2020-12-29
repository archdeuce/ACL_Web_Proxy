using ACL_Web_Proxy.Model;
using ACL_Web_Proxy.Tools;
using ACL_Web_Proxy.View;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ACL_Web_Proxy
{
    public partial class ImportWindow : Window, INotifyPropertyChanged
    {
        private string userInput;
        private int preIndex;
        private readonly Parser parser;
        private Employee selectedEmployee;
        private List<Employee> selectedEmployees;
        private ObservableCollection<Employee> Geocoders { get; set; }
        private ObservableCollection<Employee> filteredGeocoders;

        #region PropertyChanged
        public string UserInput
        {
            get
            {
                return this.userInput;
            }
            set
            {
                if (this.userInput == value)
                    return;

                //this.userInput = value;
                this.userInput = this.parser.ReplaceNonLetterSymbols(value, string.Empty);
                this.OnPropertyChanged(nameof(this.UserInput));

                this.SearchCommandExecuted(System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        public Employee SelectedEmployee
        {
            get
            {
                return this.selectedEmployee;
            }
            set
            {
                if (this.selectedEmployee == value)
                    return;

                this.selectedEmployee = value;
                this.OnPropertyChanged(nameof(this.SelectedEmployee));
            }
        }

        public List<Employee> SelectedEmployees
        {
            get
            {
                return this.selectedEmployees;
            }
            set
            {
                if (this.selectedEmployees == value)
                    return;

                this.selectedEmployees = value;
                this.OnPropertyChanged(nameof(this.SelectedEmployees));
            }
        }

        public ObservableCollection<Employee> FilteredGeocoders
        {
            get
            {
                return this.filteredGeocoders;
            }
            set
            {
                if (this.filteredGeocoders == value)
                    return;

                this.filteredGeocoders = value;
                this.OnPropertyChanged(nameof(this.FilteredGeocoders));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged is null)
                return;

            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region ICommands
        public ICommand OkButtonCommand { get; set; }
        public ICommand CancelButtonCommand { get; set; }
        public ICommand RowSelectedCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand CopyFullNameCommand { get; set; }

        #endregion

        public ImportWindow(ObservableCollection<Employee> geocoders)
        {
            this.InitializeComponent();
            this.DataContext = this;

            this.OkButtonCommand = new RelayCommand(this.OkButtonCommandExecuted, this.OkButtonCommandCanExecute);
            this.CancelButtonCommand = new RelayCommand(this.CancelButtonCommandExecuted, this.CancelButtonCommandCanExecute);
            this.RowSelectedCommand = new RelayCommand(this.RowSelectedCommandExecuted, this.RowSelectedCommandCanExecute);
            this.SearchCommand = new RelayCommand(this.SearchCommandExecuted, this.SearchCommandCanExecute);
            this.CopyFullNameCommand = new RelayCommand(this.CopyFullNameCommandExecuted, this.CopyFullNameCommandCanExecute);

            this.parser = new Parser();
            this.Geocoders = geocoders;
            this.UserInput = string.Empty;
            this.FilteredGeocoders = new ObservableCollection<Employee>(this.Geocoders);
            this.SelectedEmployees = new List<Employee>();
        }

        #region Events
        // Код, выполняемый при загрузке окна
        private void ImportWindowLoaded(object sender, RoutedEventArgs e)
        {
            this.searchTB.Focus();
        }

        // Обработчик фокуса поля ввода
        private void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            this.SetKeyboardLayout("en-US");
        }

        // Принудительная установка раскладки клавиатуры
        private void SetKeyboardLayout(string layout = "en-US")
        {
            System.Windows.Forms.InputLanguage.CurrentInputLanguage = System.Windows.Forms.InputLanguage.FromCulture(new System.Globalization.CultureInfo(layout));
        }

        // Проверка на нажатие кнопок
        private void SearchTB_PressKey(object sender, KeyEventArgs e)
        {
            // Перемещение фокуса на датагрид
            if (e.Key == Key.Down)
            {
                if (this.FilteredGeocoders.Count == 0)
                    return;

                // Если 1я строка выделена - выделяем 2ю строку
                if (this.FilteredGeocoders.Count > 1 && this.imDG.SelectedIndex == 0)
                    this.imDG.SelectedIndex = 1;
                else
                    this.imDG.SelectedIndex = 0;

                DataGridRow row = (DataGridRow)this.imDG.ItemContainerGenerator.ContainerFromIndex(this.imDG.SelectedIndex);
                row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }

            // При нажатии Ентер в поле ввода выбираем первый (существующий) элемент из датагрида
            if (e.Key == Key.Enter)
            {
                this.SelectedEmployees.Clear();
                this.SelectedEmployees.Add(this.FilteredGeocoders.FirstOrDefault());
                this.OkButtonCommandExecuted(System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        // Перемещение фокуса на поле ввода
        private void DataGrid_KeyUp(object sender, KeyEventArgs e)
        {
            // После 1го элемента в датагриде идет переход на поле ввода
            if (this.preIndex == 0 && e.Key == Key.Up)
            {
                this.imDG.SelectedIndex = -1;
                this.searchTB.Focus();
            }

            this.preIndex = this.imDG.SelectedIndex;
            string l = e.Key.ToString();

            // Мониторим только кнопки с буквами
            if (l.Length > 1)
                return;

            // При нажатии букв - возвращаем фокус и букву в поле фильтрации
            if (new Regex("[a-zA-Z]").IsMatch(l))
            {
                this.UserInput += l;
                this.searchTB.Focus();
                this.searchTB.SelectionStart = this.searchTB.Text.Length;
            }
        }
        #endregion

        #region Commands
        // Кнопка Ok
        private void OkButtonCommandExecuted(object obj)
        {
            this.DialogResult = true;
            this.Close();
        }

        private bool OkButtonCommandCanExecute(object obj)
        {
            if (this.SelectedEmployees?.Count > 0)
                return true;

            return false;
        }

        // Кнопка Cancel
        private void CancelButtonCommandExecuted(object obj)
        {
            this.DialogResult = false;
            this.Close();
        }

        private bool CancelButtonCommandCanExecute(object obj)
        {
            return true;
        }

        // Получение списка сотрудников из датагрида
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            IList selectedList = dg.SelectedItems;

            this.SelectedEmployees = selectedList.OfType<Employee>().ToList();
        }

        // По даблклику закрывает окно и добавляет выбранного сотрудника
        private void RowSelectedCommandExecuted(object obj)
        {
            this.OkButtonCommandExecuted(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private bool RowSelectedCommandCanExecute(object obj)
        {
            if (this.SelectedEmployees?.Count > 0)
                return true;

            return false;
        }

        // Фильтрация сотрудников
        public void SearchCommandExecuted(object obj)
        {
            string f = this.UserInput.Trim();

            this.FilteredGeocoders = new ObservableCollection<Employee>(this.Geocoders.Where(e => e.FirstName.ToLower().Contains(f.ToLower()) || e.LastName.ToLower().Contains(f.ToLower())).OrderBy(e => e.FirstName).ToList());

            if (!string.IsNullOrEmpty(f))
                this.imDG.SelectedIndex = 0;
        }

        public bool SearchCommandCanExecute(object obj)
        {
            if (this.Geocoders?.Count > 0 || this.FilteredGeocoders?.Count > 0)
                return true;

            return false;
        }

        // Копирование выбранного сотрудника из датагрида
        public void CopyFullNameCommandExecuted(object obj)
        {
            Clipboard.SetText($"{this.SelectedEmployee.FirstName} {this.SelectedEmployee.LastName}\n");
        }

        public bool CopyFullNameCommandCanExecute(object obj)
        {
            return this.SelectedEmployee != null;
        }
        #endregion
    }
}
