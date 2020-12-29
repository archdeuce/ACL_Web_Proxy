using ACL_Web_Proxy.Model;
using System.Collections.Generic;

namespace ACL_Web_Proxy.Services
{
    public interface IEmployeeService
    {
        IEnumerable<Log> GetLogs();
        void AddLogLine(Log log);
        void AddLogLines(IEnumerable<Log> logs);
        void SaveChanges();

        IEnumerable<Employee> GetEmployees();
        void AddEmployees(IEnumerable<Employee> Employees);
        void DeleteAllEmployees();
    }
}