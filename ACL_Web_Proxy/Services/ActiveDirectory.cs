using ACL_Web_Proxy.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace ACL_Web_Proxy.Services
{
    public class ActiveDirectory
    {
        private readonly string ldapPathCoders;
        private readonly string ldapPathStudents;
        private readonly string ldapPathPersonnel;
        private readonly string ldapPathDC;
        private readonly string ldapPathAccessGroup;

        public List<Employee> GeocoderUsers
        {
            get
            {
                return this.GetUserList(this.ldapPathCoders);
            }
        }

        public List<Employee> StudentUsers
        {
            get
            {
                return this.GetUserList(this.ldapPathStudents);
            }
        }

        public List<Employee> WebProxyUsers
        {
            get
            {
                return this.GetGroupUserList(this.ldapPathAccessGroup);
            }
        }

        public ActiveDirectory()
        {
            this.ldapPathDC = "DC=intetics,DC=com,DC=ua";
            this.ldapPathPersonnel = $"LDAP://OU=Personnel,{this.ldapPathDC}";
            this.ldapPathCoders = $"LDAP://OU=Coders,OU=Gis,OU=Personnel,{this.ldapPathDC}";
            this.ldapPathStudents = $"LDAP://OU=Students,OU=Gis,OU=Personnel,{this.ldapPathDC}";
            this.ldapPathAccessGroup = $"LDAP://OU=Access,OU=Groups,{this.ldapPathDC}";
        }

        // Поиск сотрудников по ФИО
        private Employee GetSearchedEmployee(string userLdapPath, Employee employee)
        {
            DirectoryEntry de = new DirectoryEntry
            {
                Path = userLdapPath,
            };

            DirectorySearcher ds = new DirectorySearcher(de)
            {
                Filter = $"(&(objectClass=user)(givenName={employee.FirstName})(sn={employee.LastName}))",
            };

            SearchResult sr = ds?.FindOne();
            Employee e = new Employee();

            if (sr.Properties.Contains("givenname"))
                e.FirstName = sr.Properties["givenname"][0].ToString();

            if (sr.Properties.Contains("sn"))
                e.LastName = sr.Properties["sn"][0].ToString();

            if (sr.Properties.Contains("userPrincipalName"))
                e.PrincipalName = sr.Properties["userPrincipalName"][0].ToString();

            return e;
        }

        // Получение списка сотрудников из группы
        private List<Employee> GetGroupUserList(string groupLdapPath, string groupName = DefaultValues.AdProxyGroup)
        {
            DirectoryEntry de = new DirectoryEntry
            {
                Path = groupLdapPath,
            };

            DirectorySearcher ds = new DirectorySearcher(de)
            {
                Filter = $"(&(objectClass=group)(cn={groupName}))",
            };

            SearchResultCollection src = ds?.FindAll();
            List<Employee> resultList = new List<Employee>();

            foreach (SearchResult sr in src)
            {
                if (sr.Properties.Contains("member"))
                {
                    foreach (string item in sr.Properties["member"])
                    {
                        Employee empTmp = this.GetEmployeeFromCanonicalName(item);

                        if (empTmp is null)
                            continue;

                        Employee empRes = this.GetSearchedEmployee(this.ldapPathPersonnel, empTmp);

                        resultList.Add(empRes);
                    }
                }
            }

            return resultList;
        }

        // Получение списка пользователей из выбранного юнита
        private List<Employee> GetUserList(string userLdapPath)
        {
            DirectoryEntry de = new DirectoryEntry //(domain, user, password)
            {
                AuthenticationType = AuthenticationTypes.Secure,
                Path = userLdapPath,
            };

            DirectorySearcher ds = new DirectorySearcher(de)
            {
                Filter = "(&(objectCategory=User)(objectClass=person))",
            };


            int count = 0;
            SearchResultCollection src = ds?.FindAll();

            List<Employee> resultList = new List<Employee>();

            foreach (SearchResult sr in src)
            {
                count++;
                Employee e = new Employee();

                if (sr.Properties.Contains("givenname"))
                    e.FirstName = sr.Properties["givenname"][0].ToString();

                if (sr.Properties.Contains("sn"))
                    e.LastName = sr.Properties["sn"][0].ToString();

                if (sr.Properties.Contains("userPrincipalName"))
                    e.PrincipalName = sr.Properties["userPrincipalName"][0].ToString();

                resultList.Add(e);
            }

            return resultList;
        }

        // ФИО из стремной строки
        private Employee GetEmployeeFromCanonicalName(string canonicalName)
        {
            int start = canonicalName.IndexOf('=') + 1;
            int end = canonicalName.IndexOf(',') - start;
            string fullName = canonicalName.Substring(start, end);
            string[] s = fullName.Split(' ');

            if (s.Length < 2)
                return null;

            Employee e = new Employee()
            {
                FirstName = s[0],
                LastName = s[1],
            };

            return e;
        }

        // Добавление юзера в группу
        public void AddUserToGroup(string userId, string groupName = DefaultValues.AdProxyGroup)
        {
            try
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, DefaultValues.CompanyDomain))
                {
                    try
                    {
                        GroupPrincipal group = GroupPrincipal.FindByIdentity(pc, groupName);

                        if (!group.Members.Contains(pc, IdentityType.UserPrincipalName, userId))
                        {
                            group.Members.Add(pc, IdentityType.UserPrincipalName, userId);
                            group.Save();
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Добавление юзеров в группу
        public void AddUsersToGroup(List<Employee> employeesList, string groupName = DefaultValues.AdProxyGroup)
        {
            foreach (var e in employeesList)
            {
                this.AddUserToGroup(e.PrincipalName, groupName);
            }
        }

        // Удаление юзера из группы
        public void RemoveUserFromGroup(string userId, string groupName = DefaultValues.AdProxyGroup)
        {
            try
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, DefaultValues.CompanyDomain))
                {
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(pc, groupName);

                    if (group.Members.Contains(pc, IdentityType.UserPrincipalName, userId))
                    {
                        group.Members.Remove(pc, IdentityType.UserPrincipalName, userId);
                        group.Save();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Удаление юзеров из группы
        public void RemoveUsersFromGroup(List<Employee> employeesList, string groupName = DefaultValues.AdProxyGroup)
        {
            foreach (var e in employeesList)
                this.RemoveUserFromGroup(e.PrincipalName, groupName);
        }
    }
}