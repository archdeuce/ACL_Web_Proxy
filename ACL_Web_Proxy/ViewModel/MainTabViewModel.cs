using ACL_Web_Proxy.Model;
using ACL_Web_Proxy.Services;
using ACL_Web_Proxy.Tools;
using ACL_Web_Proxy.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ACL_Web_Proxy.ViewModel
{
    public class MainTabViewModel : BaseViewModel, INotifyPropertyChanged
    {
        #region Fields and Properties
        private bool isLoaded;
        private OsInfo osInfo;
        private readonly Parser parser;
        private readonly ActiveDirectory ad;
        private bool isNeedApply;
        private string userInput;
        private DateTime selectedDate;
        private Employee selectedEmployee;
        private List<Employee> selectedEmployees;
        private readonly List<Employee> addedEmployeeList;
        private readonly List<Employee> updatedEmployeeList;
        private readonly List<Employee> adEmployeesForRemove;
        private readonly ObservableCollection<Employee> adEmployeesAdmins;
        private ObservableCollection<Employee> dbEmployees;
        private ObservableCollection<Employee> adEmployees;
        private ObservableCollection<Employee> adEmployeesGeocoders;
        #endregion

        #region PropertyChanged
        public ObservableCollection<Employee> DbEmployees
        {
            get
            {
                return this.dbEmployees;
            }
            set
            {
                if (this.dbEmployees == value)
                    return;

                this.dbEmployees = value;

                this.OnPropertyChanged(nameof(this.DbEmployees));
            }
        }

        public ObservableCollection<Employee> AdEmployees
        {
            get
            {
                return this.adEmployees;
            }
            set
            {
                if (this.adEmployees == value)
                    return;

                this.adEmployees = value;

                this.OnPropertyChanged(nameof(this.AdEmployees));
            }
        }

        public ObservableCollection<Employee> AdEmployeesGeocoders
        {
            get
            {
                return this.adEmployeesGeocoders;
            }
            set
            {
                if (this.adEmployeesGeocoders == value)
                    return;

                this.adEmployeesGeocoders = value;

                this.OnPropertyChanged(nameof(this.AdEmployeesGeocoders));
            }
        }

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

                this.userInput = value;
                this.OnPropertyChanged(nameof(this.UserInput));
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

        public DateTime SelectedDate
        {
            get
            {
                return this.selectedDate;
            }
            set
            {
                if (this.selectedDate == value)
                    return;

                this.selectedDate = value;
                this.OnPropertyChanged(nameof(this.SelectedDate));
            }
        }
        #endregion

        #region ICommands
        public ICommand ImportEmployeesCommand { get; set; }
        public ICommand AddEmployeesCommand { get; set; }
        public ICommand RemoveExpiredCommand { get; set; }
        public ICommand ApplyCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
        public ICommand CopyFullNameCommand { get; set; }
        public ICommand DeleteEmployeeCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }
        #endregion

        #region Constructor
        public MainTabViewModel(IEmployeeService employeeService)
        {
            this.ImportEmployeesCommand = new RelayCommand(this.ImportEmployeesCommandExecuted, this.ImportEmployeesCommandCanExecute);
            this.AddEmployeesCommand = new RelayCommand(this.AddEmployeesCommandExecuted, this.AddEmployeesCommandCanExecute);
            this.RemoveExpiredCommand = new RelayCommand(this.RemoveExpiredCommandExecuted, this.RemoveExpiredCommandCanExecute);
            this.ApplyCommand = new RelayCommand(this.ApplyCommandExecuted, this.ApplyCommandCanExecute);
            this.RefreshCommand = new RelayCommand(this.RefreshCommandExecuted, this.RefreshCommandCanExecute);
            this.LoadedCommand = new RelayCommand(this.LoadedCommandExecuted, this.LoadedCommandCanExecute);
            this.CopyFullNameCommand = new RelayCommand(this.CopyFullNameCommandExecuted, this.CopyFullNameCommandCanExecute);
            this.DeleteEmployeeCommand = new RelayCommand(this.DeleteEmployeeCommandExecuted, this.DeleteEmployeeCommandCanExecute);
            this.SelectionChangedCommand = new RelayCommand(this.SelectionChangedCommandExecuted, this.SelectionChangedCommandCanExecute);

            this.ad = new ActiveDirectory();
            this.OsInfo = new OsInfo();
            this.parser = new Parser();

            this.employeeService = employeeService;

            if (this.employeeService == null)
                throw new ArgumentNullException(nameof(this.employeeService));

            this.DbEmployees = new ObservableCollection<Employee>();
            this.AdEmployees = new ObservableCollection<Employee>();
            this.adEmployeesAdmins = new ObservableCollection<Employee>();
            this.AdEmployeesGeocoders = new ObservableCollection<Employee>();
            this.adEmployeesForRemove = new List<Employee>();
            this.addedEmployeeList = new List<Employee>();
            this.updatedEmployeeList = new List<Employee>();

            this.SelectedDate = DateTime.Now;
            this.UserInput = string.Empty;
            this.isNeedApply = false;
            this.isLoaded = false;
        }
        #endregion

        #region Methods
        // Принудительная установка раскладки клавиатуры
        private void SetKeyboardLayout(string layout = "en-US")
        {
            System.Windows.Forms.InputLanguage.CurrentInputLanguage = System.Windows.Forms.InputLanguage.FromCulture(new System.Globalization.CultureInfo(layout));
        }

        // Обновление или установка даты сотрудникам в списке
        private void UpdateEmployeesDatesFromDb()
        {
            foreach (var adUser in this.AdEmployees)
            {
                foreach (var dbUser in this.DbEmployees)
                {
                    if (adUser.FirstName == dbUser.FirstName && adUser.LastName == dbUser.LastName)
                    {
                        adUser.ExpireDate = dbUser.ExpireDate;
                        break;
                    }
                }
            }
        }

        // Возвращение ФИО c разделителем или без
        private string ReturnFullName(Employee e, string delimiter = "\n")
        {
            if (e is null)
                return string.Empty;

            return e.FirstName + " " + e.LastName + delimiter;
        }

        private string GetEmployeesStringList(List<Employee> employeesList)
        {
            List<Employee> l = new List<Employee>(employeesList.OrderBy(e => e.FirstName));
            string result = string.Empty;

            foreach (var e in l)
                result += this.ReturnFullName(e);

            return result;
        }

        // Перебор списка импортированный пользователей и добавление в окно ввода для последующей обработки
        private void AddImportedEmployees(List<Employee> importedEmployees)
        {
            string importedStr;
            string separator = "\n";

            char lastChar = '\0';

            if (this.UserInput.Length > 0)
                lastChar = this.UserInput[this.UserInput.Length - 1];

            if (lastChar == '\r' || lastChar == '\n' || lastChar == '\0')
                importedStr = string.Empty;
            else
                importedStr = separator;

            foreach (Employee e in importedEmployees)
            {
                importedStr += $"{e.FirstName} {e.LastName}{separator}";
            }

            this.UserInput += importedStr;
        }

        // Закрытие приложения
        private void CloseApplication()
        {
            Application.Current.Shutdown();
        }

        // Получение сотрудников из базы
        private void GetDbEmployees()
        {
            this.DbEmployees = this.employeeService.GetEmployees().ToObservableCollection();
        }

        // Запись в лог того кто запустил ПО
        private void WriteUserLogin()
        {
            Log userLog = new Log()
            {
                EventDate = DateTime.Now,
                Executor = this.OsInfo.CurrentUser,
                Employee = string.Empty,
                Host = this.OsInfo.MachineName,
                Operation = "Launched a program."
            };

            this.employeeService.AddLogLine(userLog);
            this.employeeService.SaveChanges();
        }

        // Запись внесенных изменений в программе в лог
        private void WriteUserChanges(List<Employee> employees, string operation = "")
        {
            List<Log> logList = new List<Log>();

            foreach (var e in employees)
            {
                Log logLine = new Log()
                {
                    EventDate = DateTime.Now,
                    Executor = this.OsInfo.CurrentUser,
                    Employee = $"{e.FirstName} {e.LastName}",
                    Host = this.OsInfo.MachineName,
                    Operation = operation,
                    ExpireDate = e.ExpireDate
                };

                logList.Add(logLine);
            }

            this.employeeService.AddLogLines(logList);
        }
        #endregion

        #region Commands
        // Код, выполняемый при загрузке UserControl-a
        private void LoadedCommandExecuted(object obj)
        {
            this.SetKeyboardLayout("en-US");

            if (!this.isLoaded)
            { 
                this.WriteUserLogin();
                this.isLoaded = true;
                new Task(() => this.RefreshCommandExecuted(System.Reflection.MethodBase.GetCurrentMethod().Name)).Start();
            }
        }

        private bool LoadedCommandCanExecute(object obj)
        {
            return true;
        }

        // Вызов окна импорта списка сотрудников из AD
        public void ImportEmployeesCommandExecuted(object obj)
        {
            ImportWindow iw = new ImportWindow(this.AdEmployeesGeocoders)
            {
                Owner = Application.Current.MainWindow,
            };

            bool? iwResult = iw.ShowDialog().Value;
            List<Employee> importedEmployees = iw.SelectedEmployees;

            if (iwResult == true && importedEmployees?.Count > 0)
                this.AddImportedEmployees(importedEmployees);
        }

        public bool ImportEmployeesCommandCanExecute(object obj)
        {
            return true;
        }

        // Добавление сотрудников в основной список
        public void AddEmployeesCommandExecuted(object obj)
        {
            // Обновление поля ввода для наглядного примера каких пользователей навводили
            this.UserInput = this.parser.StringToColumn(this.UserInput);

            // Собираем статистику по обработке сотрудников при добавлении
            int notFoundCounter = 0;
            int updatedCounter = 0;
            int ignoredCounter = 0;
            int addedCounter = 0;
            string delimiter = "\n";
            string resultMsg = string.Empty;
            string notFound = string.Empty;
            List<Employee> ignoredList = new List<Employee>();
            List<Employee> userInputEmployeesList = this.parser.GetEmployeesList(this.UserInput);

            // Начинаем перебор сотрудников из введенного пользователем списка
            foreach (Employee inputUser in userInputEmployeesList)
            {
                bool isFinded = false;

                // Сначало пробуем найти сопадения в текущем списке
                foreach (Employee adUser in this.AdEmployees)
                {
                    if ((inputUser.FirstName == adUser.FirstName && inputUser.LastName == adUser.LastName) ||
                         (inputUser.FirstName == adUser.LastName && inputUser.LastName == adUser.FirstName))
                    {
                        if (adUser.ExpireDate == null || this.SelectedDate.Date > adUser.ExpireDate)
                        {
                            adUser.ExpireDate = this.SelectedDate.Date;
                            updatedCounter++;
                            this.updatedEmployeeList.Add(adUser);
                        }
                        else
                        {
                            ignoredCounter++;
                            ignoredList.Add(adUser);
                        }

                        isFinded = true;
                        break;
                    }
                }

                if (isFinded == true)
                    continue;

                // Потом пробуем найти сопадения в AD и(или) добавать оттуда пользователя
                foreach (Employee adUser in this.AdEmployeesGeocoders)
                {
                    if ((inputUser.FirstName == adUser.FirstName && inputUser.LastName == adUser.LastName) ||
                         (inputUser.FirstName == adUser.LastName && inputUser.LastName == adUser.FirstName))
                    {
                        if (adUser.ExpireDate == null || this.SelectedDate.Date > adUser.ExpireDate)
                        {
                            adUser.ExpireDate = this.SelectedDate.Date;
                        }
                        else
                        {
                            ignoredCounter++;
                            ignoredList.Add(adUser);
                        }

                        addedCounter++;
                        this.addedEmployeeList.Add(adUser);
                        this.AdEmployees.Add(adUser);

                        isFinded = true;
                        break;
                    }
                }

                if (isFinded == false)
                {
                    notFoundCounter++;
                    notFound += this.ReturnFullName(inputUser);
                }
            }

            // Добавление информации о добавленных сотрудниках
            if (addedCounter > 0)
            {
                resultMsg += "Добавлен(ы):" + delimiter + this.GetEmployeesStringList(this.addedEmployeeList) + delimiter;
                this.isNeedApply = true;
            }

            // Добавление информации об сотрудниках которые не были найдены
            if (notFoundCounter > 0)
            {
                resultMsg += "Не найден(ы):" + delimiter + notFound + delimiter;
                this.UserInput = notFound.Trim();
            }

            // Добавление информации об сотрудниках которым обновили в списке данные
            if (updatedCounter > 0)
            {
                resultMsg += "Обновлен(ы):" + delimiter + this.GetEmployeesStringList(this.updatedEmployeeList) + delimiter;
                this.isNeedApply = true;
            }

            // Добавление информации об сотрудниках которым не обновили в списке данные
            if (ignoredCounter > 0)
            {
                resultMsg += "Проигнорирован(ы):" + delimiter + this.GetEmployeesStringList(ignoredList) + delimiter;
            }

            // Чистим поле ввода, после успешной обработки всего пользовательского списка
            if (notFoundCounter == 0)
                this.UserInput = string.Empty;

            // Показ собранной статистики
            new Announcer().Show(resultMsg, $"Information");
        }

        public bool AddEmployeesCommandCanExecute(object obj)
        {
            // Проверка на адекватность ввода. Было бы неплохо вообще чистить лишние символы перед отправкой на формирование списка, а не по-тихому скипать их
            return this.UserInput?.Trim().Split().Length > 1;
        }

        // Удаление сотрудников из списка, у которых истек срок доступа в интернет
        public void RemoveExpiredCommandExecuted(object obj)
        {
            ObservableCollection<Employee> newEmployees = new ObservableCollection<Employee>();

            foreach (var e in this.AdEmployees)
            {
                if (e.ExpireDate == null || e.ExpireDate.Value.Date >= DateTime.Now.Date)
                    newEmployees.Add(e);
                else
                    this.adEmployeesForRemove.Add(e);
            }

            this.AdEmployees = new ObservableCollection<Employee>(newEmployees);
            this.isNeedApply = true;
        }

        public bool RemoveExpiredCommandCanExecute(object obj)
        {
            // Смотрим если кого удалять
            if (this.AdEmployees is null)
                return false;

            foreach (var emp in this.AdEmployees)
            {
                if (emp.ExpireDate != null && emp.ExpireDate < DateTime.Now.Date)
                    return true;
            }

            // Если не найдено просроченых сотрудников
            return false;
        }

        // Применение изменений и загрузка информации в БД
        public void ApplyCommandExecuted(object obj)
        {
            var res = new Announcer().Question($"Применить изменения?", "Применить?");

            if (res != System.Windows.MessageBoxResult.OK)
                return;

            // Удаляем старых и записываем в лог
            try
            {
                this.WriteUserChanges(this.adEmployeesForRemove, "User removed.");
                this.ad.RemoveUsersFromGroup(this.adEmployeesForRemove);
            }
            catch (Exception e)
            {
                new Announcer().Show(e, "Cannot delete user from Active Directory group!");
                this.CloseApplication();
            }

            // Добавляем новых
            try
            {
                this.WriteUserChanges(new List<Employee>(this.updatedEmployeeList), "User updated.");
                this.WriteUserChanges(new List<Employee>(this.addedEmployeeList), "User added.");
                this.ad.AddUsersToGroup(new List<Employee>(this.AdEmployees));
            }
            catch (Exception e)
            {
                new Announcer().Show(e, "Cannot add user to Active Directory group!");
                this.CloseApplication();
            }

            // Обновляем информацию в БД
            this.employeeService.DeleteAllEmployees();
            this.employeeService.AddEmployees(this.AdEmployees.OrderBy(e => e.FirstName));
            this.employeeService.SaveChanges();
            this.isNeedApply = false;

            // Перезагружаем актуальные данные после сохранения
            this.RefreshCommandExecuted(System.Reflection.MethodBase.GetCurrentMethod().Name);

            // Сообщаем об успешном применении изменений
            new Announcer().Show("Изменения применены.", "Information");
        }

        public bool ApplyCommandCanExecute(object obj)
        {
            // Проверка на наличие изменений в коллекции
            return this.isNeedApply;
        }

        // Получение актуальной информации с AD и БД
        public void RefreshCommandExecuted(object obj)
        {
            this.UserInput = string.Empty;
            this.adEmployeesForRemove.Clear();
            this.addedEmployeeList.Clear();
            this.updatedEmployeeList.Clear();

            Task taskDB = new Task(() =>
            {
                this.GetDbEmployees();
            });
            taskDB.Start();

            Task taskADE = new Task(() =>
            {
                this.AdEmployees = new ObservableCollection<Employee>(this.ad.WebProxyUsers.OrderBy(e => e.FirstName));
            });
            taskADE.Start();

            //Task.Run(() => this.AdEmployeesGeocoders = new ObservableCollection<Employee>(this.ad.GeocoderUsers.OrderBy(e => e.FirstName)));
            Task.Run(() => this.AdEmployeesGeocoders = new ObservableCollection<Employee>(this.ad.GeocoderUsers.Union<Employee>(this.ad.StudentUsers).OrderBy(e => e.FirstName)));

            Task.Run(() =>
            {
                taskADE.Wait();
                taskDB.Wait();
                this.UpdateEmployeesDatesFromDb();
            });

            if (this.isNeedApply)
                new Announcer().Show("Изменения не применены.", "Information");

            this.isNeedApply = false;
        }

        public bool RefreshCommandCanExecute(object obj)
        {
            return true;
        }

        // Копирование выбранного сотрудника из датагрида
        public void CopyFullNameCommandExecuted(object obj)
        {
            string result = string.Empty;

            if (this.SelectedEmployees == null)
            {
               result += $"{this.SelectedEmployee.FirstName} {this.SelectedEmployee.LastName}\n";
            }
            else
            {
                foreach (Employee e in this.SelectedEmployees)
                {
                    result += $"{e.FirstName} {e.LastName}\n";
                }
            }

            Clipboard.SetText(result);
        }

        public bool CopyFullNameCommandCanExecute(object obj)
        {
            return this.SelectedEmployee != null || this.SelectedEmployees != null;
        }

        // Удаление выбранного сотрудника из датагрида
        public void DeleteEmployeeCommandExecuted(object obj)
        {
            var res = new Announcer().Question($"Удалить выбранных сотрудников?");

            if (res != System.Windows.MessageBoxResult.OK)
                return;

            // Добавляем сотрудников в очередь на удаление и удаляем из датагрида
            if (this.SelectedEmployees == null)
            {
                this.adEmployeesForRemove.Add(this.SelectedEmployee);
                this.AdEmployees.Remove(this.SelectedEmployee);
            }
            else
            {
                foreach (Employee e in this.SelectedEmployees)
                {
                    this.adEmployeesForRemove.Add(e);
                    this.AdEmployees.Remove(e);
                }
            }

            this.isNeedApply = true;
        }

        public bool DeleteEmployeeCommandCanExecute(object obj)
        {
            return this.SelectedEmployee != null || this.SelectedEmployees != null;
            //return this.SelectedEmployees?.Count > 0;
        }

        // Получение данных о выбранных сотрудников из DataGrid
        private void SelectionChangedCommandExecuted(object SelectedItems)
        {
            this.SelectedEmployees = ((IEnumerable)SelectedItems).Cast<Employee>().ToList();
        }

        private bool SelectionChangedCommandCanExecute(object SelectedItems)
        {
            return true;
        }
        #endregion
    }
}
