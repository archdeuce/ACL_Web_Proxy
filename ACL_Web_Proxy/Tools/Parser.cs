using ACL_Web_Proxy.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ACL_Web_Proxy.Tools
{
    public class Parser
    {
        // Возвращает список обьектов-сотрудников
        public List<Employee> GetEmployeesList(string users)
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

        public string StringToColumn(string str)
        {
            string[] tmp = this.RemoveMultispaces(this.ReplaceNonLetterSymbols(str)).Split(' ');
            string res = tmp[0];

            for (int i = 1; i < tmp.Length; i++)
            {
                if (i % 2 == 1)
                    res += " ";
                else
                    res += "\r\n";

                res += tmp[i];
            }

            return res;
        }

        // Replace characters in a row are not letters.
        public string ReplaceNonLetterSymbols(string str, string symbol = " ")
        {
            if (str is null)
                return string.Empty;

            Regex rgx = new Regex("[^a-zA-Zа-яА-Я -]");
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