using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System.Globalization;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            string result = RemoveTown(context);

            Console.WriteLine(result);
        }

        //Problem 03
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees =
                context.Employees
                .OrderBy(e => e.EmployeeId)
                .AsNoTracking()
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary
                })
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine
                    ($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
            }

            return sb.ToString().Trim();
        }

        // Problem 04
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees =
                context.Employees
                .AsNoTracking()
                .Where(e => e.Salary > 50000)
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }

            return sb.ToString().Trim();
        }

        //Problem 05
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees =
                context.Employees
                .AsNoTracking()
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    DepartmentName = e.Department.Name,
                    e.Salary
                })
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:f2}");
            }

            return sb.ToString().Trim();
        }

        //Problem 06
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address newAddress = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            Employee employee = context.Employees.FirstOrDefault(e => e.LastName == "Nakov");

            employee!.Address = newAddress;

            context.SaveChanges();

            StringBuilder sb = new StringBuilder();

            var employeesAddresses = context.Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => e.Address!.AddressText)
                .ToArray();

            foreach (var e in employeesAddresses)
            {
                sb.AppendLine(e);
            }

            return sb.ToString().Trim();
        }

        //Problem 07
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employeesWithProjects = context.Employees
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager!.FirstName,
                    ManagerLastName = e.Manager!.LastName,
                    Projects = e.EmployeesProjects
                        .Where(ep => ep.Project.StartDate.Year >= 2001 &&
                                     ep.Project.StartDate.Year <= 2003)
                        .Select(ep => new
                        {
                            ProjecName = ep.Project.Name,
                            StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                            EndDate = ep.Project.EndDate.HasValue ?
                                      ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture) : "not finished"
                        })
                        .ToArray()
                })
                .ToArray();

            foreach (var e in employeesWithProjects)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");

                foreach (var p in e.Projects)
                {
                    sb.AppendLine($"--{p.ProjecName} - {p.StartDate} - {p.EndDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 08
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var addresses =
                context.Addresses
                .AsNoTracking()
                .Select(a => new
                {
                    a.AddressText,
                    Town = a.Town!.Name,
                    Employees = a.Employees.Count
                })
                .OrderByDescending(a => a.Employees)
                .ThenBy(a => a.Town)
                .ThenByDescending(a => a.AddressText)
                .ToArray();

            foreach (var a in addresses)
            {
                sb.AppendLine($"{a.AddressText}, {a.Town} - {a.Employees} employees");
            }

            return sb.ToString().Trim();
        }

        //Problem 09
        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employee = context.Employees
                .AsNoTracking()
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    Projects = e.EmployeesProjects
                        .OrderBy(ep => ep.Project.Name)
                        .Select(ep => new
                        {
                            ep.Project.Name
                        })
                        .ToArray()
                })
                .First();

            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

            foreach (var p in employee.Projects)
            {
                sb.AppendLine($"{p.Name}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 10
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employee =
                context.Departments
                .AsNoTracking()
                .Where(d => d.Employees.Count > 5)
                .Select(d => new
                {
                    d.Name,
                    ManagerFirstName = d.Manager.FirstName,
                    ManagerLastName = d.Manager.LastName,
                    EmployeeInThisDepartment = d.Employees
                    .Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        e.JobTitle
                    })
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .ToArray()
                })
                .ToArray();

            foreach (var d in employee)
            {
                sb.AppendLine($"{d.Name} - {d.ManagerFirstName} {d.ManagerLastName}");

                foreach (var e in d.EmployeeInThisDepartment)
                {
                    sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                }
            }

            return sb.ToString().Trim();
        }

        // Problem 11
        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var lastTenProjects =
                context.Projects
                .AsNoTracking()
                .Take(10)
                .OrderByDescending(p => p.StartDate)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    StartDate =
                    p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                })
                .ToArray();

            foreach (var p in lastTenProjects.OrderBy(x => x.Name))
            {
                sb.AppendLine(p.Name)
                  .AppendLine(p.Description)
                  .AppendLine(p.StartDate);
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 12
        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employeesIncreseSalary =
                context.Employees
                .AsNoTracking()
                .Where(e => e.Department.Name == "Engineering" ||
                        e.Department.Name == "Engineering" ||
                        e.Department.Name == "Tool Design" ||
                        e.Department.Name == "Marketing" ||
                        e.Department.Name == "Information Services")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            foreach (var e in employeesIncreseSalary)
            {
                e.Salary += e.Salary * 0.12m;
            }

            foreach (var employee in employeesIncreseSalary)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} (${employee.Salary:f2})");
            }

            return sb.ToString().Trim();
        }

        //Problem 13
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employee =
                context.Employees
                .AsNoTracking()
                .Where(e => e.FirstName.StartsWith("Sa"))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToArray();

            foreach (var e in employee)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})");
            }

            return sb.ToString().Trim();
        }

        // Problem 14
        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var deleteProject = context.EmployeesProjects.Where(p => p.ProjectId == 2);
            context.RemoveRange(deleteProject);

            var projectToDelete = context.Projects.Find(2);
            context.Remove(projectToDelete);

            context.SaveChanges();

            var takeTenProjects =
                context.Projects
                .AsNoTracking()
                .Take(10)
                .Select(p => p.Name)
                .ToArray();

            foreach (var p in takeTenProjects)
            {
                sb.AppendLine(p);
            }

            return sb.ToString().Trim();
        }

        // Problem 15
        public static string RemoveTown(SoftUniContext context)
        {
            var townToDelete =
                context.Towns
                .Where(t => t.Name == "Seattle")
                .FirstOrDefault();

            var refferedAddresses =
                context.Addresses
                .Where(a => a.TownId == townToDelete!.TownId)
                .ToList();

            foreach (var e in context.Employees)
            {
                if (refferedAddresses.Any(x => x.AddressId == e.AddressId))
                {
                    e.AddressId = null;
                }
            }

            var numberOfAddressesDeleted = refferedAddresses.Count;


            context.Addresses.RemoveRange(refferedAddresses);
            context.Towns.Remove(townToDelete!);

            context.SaveChanges();

            return $"{numberOfAddressesDeleted} addresses in Seattle were deleted";
        }
    }
}