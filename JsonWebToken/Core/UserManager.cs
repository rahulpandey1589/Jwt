using JsonWebToken.Models;
using JsonWebToken.Token;
using System.Collections.Generic;
using System.Linq;

namespace JsonWebToken.Core
{
    public class UserManager
    {
        public string ValidateUser(string userName, string password)
        {
            if (userName == "Rahul" && password.Equals(password))
            {
                return TokenManager.GenerateToken(userName);
            }
            return null;
        }



        public Employee FindByName(int employeeId)
        {
            return BindAllEmployees().Where(x => x.EmployeeId.Equals(employeeId)).FirstOrDefault();
        }


        private IEnumerable<Employee> BindAllEmployees()
        {
            return new List<Employee>()
            {
                new Employee()
                {
                    EmployeeId = 1,
                    FullName="Rahul Pandey",
                    Gender="M"
                },
                new Employee()
                {
                    EmployeeId = 2,
                    FullName="Rajneesh Pandey",
                    Gender="M"
                },
                new Employee()
                {
                    EmployeeId = 3,
                    FullName="Surendra Pandey",
                    Gender="M"
                },
                new Employee()
                {
                    EmployeeId = 4,
                    FullName="Saraswati Pandey",
                    Gender="M"
                }
            };
        }
    }
}