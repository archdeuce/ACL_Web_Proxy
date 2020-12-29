using ACL_Web_Proxy.Model;
using System.Collections.Generic;
using System.Linq;

namespace ACL_Web_Proxy.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly EmployeeDbContext context;

        public EmployeeService()
        {
            this.context = new EmployeeDbContext();
        }

        // Сохранение изменений
        public void SaveChanges()
        {
            this.context.SaveChanges();
        }

        //
        // Работа с логом
        //
        public IEnumerable<Log> GetLogs()
        {
            return new List<Log>(this.context.Logs);
        }

        // Запись строки в лог
        public void AddLogLine(Log log)
        {
            this.context.Add(log);
        }

        // Запись строк в лог
        public void AddLogLines(IEnumerable<Log> logs)
        {
            this.context.AddRange(logs);
        }

        //
        // Работа с сотрудниками
        //
        public IEnumerable<Employee> GetEmployees()
        {
            return new List<Employee>(this.context.Employees);
        }

        public void DeleteAllEmployees()
        {
            this.context.RemoveRange(this.context.Employees.ToList());
        }

        public void AddEmployees(IEnumerable<Employee> Employees)
        {
            this.context.UpdateRange(Employees);
        }
    }
}