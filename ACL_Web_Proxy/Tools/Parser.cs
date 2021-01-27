using ACL_Web_Proxy.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ACL_Web_Proxy.Tools
{
    public class Parser
    {
        // Возвращает список обьектов-сотрудников с именами
        public List<Employee> GetEmployeesListByName(string users)
        {
            List<string> words = this.RemoveMultispaces(this.ReplaceNonLetterSymbols(users)).Split(' ').ToList();
            List<Employee> userList = new List<Employee>();
            Employee u = new Employee();

            foreach (string word in words)
            {
                if (u.FirstName is null)
                {
                    u.FirstName = word;
                }
                else if (u.LastName is null)
                {
                    u.LastName = word;
                    userList.Add(u);
                    u = new Employee();
                }
            }

            return userList;
        }

        // Возвращает список обьектов-сотрудников с почтой
        public List<Employee> GetEmployeesListByEmail(string users)
        {
            List<string> emails = this.RemoveMultispaces(this.ReplaceNonLetterSymbols(users, exc: "@.")).Split(' ').ToList();
            List<Employee> userList = new List<Employee>();

            foreach (string email in emails)
            {
                userList.Add(new Employee() { PrincipalName = email });
            }

            return userList;
        }

        public string StringToColumn(string str)
        {
            string res = string.Empty;
            string[] tmp;

            if (str.Contains('@'))
            {
                tmp = this.RemoveMultispaces(this.ReplaceNonLetterSymbols(str, exc: "@.")).Split(' ');
                res = string.Join("\r\n", tmp);
            }
            else 
            {
                tmp = this.RemoveMultispaces(this.ReplaceNonLetterSymbols(str)).Split(' ');
                res = tmp[0];

                for (int i = 1; i < tmp.Length; i++)
                {
                    if (i % 2 == 1)
                        res += " ";
                    else
                        res += "\r\n";

                    res += tmp[i];
                }
            }

            return res;
        }

        // Replace characters in a row are not letters.
        public string ReplaceNonLetterSymbols(string str, string symbol = " ", string exc = "")
        {
            if (str is null)
                return string.Empty;

            Regex rgx = new Regex($"[^a-zA-Zа-яА-Я{exc} -]");
            return str = rgx.Replace(str, symbol);
        }

        // Removal of any kind of whitespace (tabs, new line, etc.) and replace them with a single space.
        public string RemoveMultispaces(string str)
        {
            // while (str.Contains("  ")) str = str.Replace("  ", " ");
            return Regex.Replace(str, @"\s+", " ").Trim();
        }
    }
}