using System;
using System.Collections.Generic;
using System.Linq;
using LowDBNet;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("LowDB.NET Advanced Demo");
        Console.WriteLine("=======================");

        var db = new LowDB("advancedtestdb.json");
        var departments = db.GetCollection<Department>("departments");

        Console.WriteLine($"Number of departments: {departments.Count()}");

        var s = db.GetCollection<Department>("departments").ToList();

        DisplayAllDepartments(departments);
        PerformComplexQueries(departments);
        UpdateNestedData(db);
        RemoveComplexData(db);

        db.SaveChanges();

        Console.WriteLine("\nAll operations have been saved to the database.");
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    static void DisplayAllDepartments(IEnumerable<Department> departments)
    {
        Console.WriteLine("\nAll Departments and Employees:");
        foreach (var dept in departments)
        {
            Console.WriteLine($"Department: {dept.Name}");
            if (dept.Manager != null)
            {
                Console.WriteLine($"Manager: {dept.Manager.Name}");
            }
            else
            {
                Console.WriteLine("Manager: Not assigned");
            }
            Console.WriteLine("Employees:");
            if (dept.Employees != null)
            {
                foreach (var emp in dept.Employees)
                {
                    Console.WriteLine($"  - {emp.Name} ({string.Join(", ", emp.Skills ?? new List<string>())})");
                }
            }
            else
            {
                Console.WriteLine("  No employees");
            }
            Console.WriteLine("Projects:");
            if (dept.Projects != null)
            {
                foreach (var proj in dept.Projects)
                {
                    Console.WriteLine($"  - {proj.Name} (Budget: ${proj.Budget:N0})");
                }
            }
            else
            {
                Console.WriteLine("  No projects");
            }
            Console.WriteLine();
        }
    }

    static void PerformComplexQueries(IEnumerable<Department> departments)
    {
        Console.WriteLine("\nComplex Queries:");

        // Find departments with projects having a budget over $100,000
        var highBudgetDepts = departments.Where(d => d.Projects != null && d.Projects.Any(p => p.Budget > 100000));
        Console.WriteLine($"Departments with high-budget projects: {string.Join(", ", highBudgetDepts.Select(d => d.Name))}");

        // Find employees with C# skills across all departments
        var csharpDevs = departments
            .SelectMany(d => d.Employees ?? Enumerable.Empty<Employee>())
            .Where(e => e.Skills != null && e.Skills.Contains("C#"))
            .ToList();
        Console.WriteLine($"C# developers: {string.Join(", ", csharpDevs.Select(e => e.Name))}");

        // Find the department with the most employees
        var largestDept = departments.OrderByDescending(d => d.Employees?.Count ?? 0).FirstOrDefault();
        if (largestDept != null)
        {
            Console.WriteLine($"Largest department: {largestDept.Name} with {largestDept.Employees?.Count ?? 0} employees");
        }
        else
        {
            Console.WriteLine("No departments found");
        }

        // Calculate total budget across all projects
        var totalBudget = departments
            .SelectMany(d => d.Projects ?? Enumerable.Empty<Project>())
            .Sum(p => p.Budget);
        Console.WriteLine($"Total budget across all projects: ${totalBudget:N0}");
    }

    static void UpdateNestedData(LowDB db)
    {
        Console.WriteLine("\nUpdating Nested Data:");

        var departments = db.GetCollection<Department>("departments");
        var engineeringDept = departments.FirstOrDefault(d => d.Name == "Engineering");
        if (engineeringDept != null)
        {
            var alice = engineeringDept.Employees?.FirstOrDefault(e => e.Name == "Alice Smith");
            if (alice != null)
            {
                alice.Skills ??= new List<string>();
                alice.Skills.Add("React");
                db.UpdateInCollection("departments", d => d.Id == engineeringDept.Id, engineeringDept);
                Console.WriteLine("Added 'React' skill to Alice Smith.");
            }
        }

        var marketingDept = departments.FirstOrDefault(d => d.Name == "Marketing");
        if (marketingDept != null)
        {
            var brandCampaign = marketingDept.Projects?.FirstOrDefault(p => p.Name == "Brand Campaign");
            if (brandCampaign != null)
            {
                brandCampaign.Budget *= 1.1m; // 10% increase
                db.UpdateInCollection("departments", d => d.Id == marketingDept.Id, marketingDept);
                Console.WriteLine($"Increased budget for Brand Campaign to ${brandCampaign.Budget:N0}");
            }
        }
    }

    static void RemoveComplexData(LowDB db)
    {
        Console.WriteLine("\nRemoving Complex Data:");

        var departments = db.GetCollection<Department>("departments").ToList();
        var oneMonthAgo = DateTime.Now.AddMonths(-1);

        foreach (var dept in departments)
        {
            if (dept.Projects != null)
            {
                dept.Projects.RemoveAll(p => p.StartDate < oneMonthAgo);
            }
            if (dept.Employees != null)
            {
                dept.Employees.RemoveAll(e => e.Skills == null || e.Skills.Count == 0);
            }
            db.UpdateInCollection("departments", d => d.Id == dept.Id, dept);
        }
        Console.WriteLine("Removed all projects started more than a month ago and employees with no skills.");
    }
}

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Employee Manager { get; set; }
    public List<Employee> Employees { get; set; }
    public List<Project> Projects { get; set; }
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<string> Skills { get; set; }
}

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Budget { get; set; }
    public DateTime StartDate { get; set; }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using LowDBNet;

//class Program
//{
//    static void Main(string[] args)
//    {
//        Console.WriteLine("LowDB.NET Advanced Demo");
//        Console.WriteLine("=======================");

//        // Initialize the database with the JSON file
//        var db = new LowDB("advancedtestdb.json");

//        // Get the departments collection
//        var departments = db.GetCollection<Department>("departments");

//        // Debug: Print the number of departments
//        Console.WriteLine($"Number of departments: {departments.Find(d => true).Count()}");

//        // Display all departments and their employees
//        DisplayAllDepartments(departments);

//        // Perform complex queries
//        PerformComplexQueries(departments);

//        // Update nested data
//        UpdateNestedData(departments);

//        // Remove data based on complex conditions
//        RemoveComplexData(departments);

//        Console.WriteLine("\nAll operations have been automatically saved to the database.");

//        Console.WriteLine("\nPress any key to exit...");
//        Console.ReadKey();
//    }

//    static void DisplayAllDepartments(Collection<Department> departments)
//    {
//        Console.WriteLine("\nAll Departments and Employees:");
//        foreach (var dept in departments.Find(d => true))
//        {
//            Console.WriteLine($"Department: {dept.Name}");
//            if (dept.Manager != null)
//            {
//                Console.WriteLine($"Manager: {dept.Manager.Name}");
//            }
//            else
//            {
//                Console.WriteLine("Manager: Not assigned");
//            }
//            Console.WriteLine("Employees:");
//            if (dept.Employees != null)
//            {
//                foreach (var emp in dept.Employees)
//                {
//                    Console.WriteLine($"  - {emp.Name} ({string.Join(", ", emp.Skills ?? new List<string>())})");
//                }
//            }
//            else
//            {
//                Console.WriteLine("  No employees");
//            }
//            Console.WriteLine("Projects:");
//            if (dept.Projects != null)
//            {
//                foreach (var proj in dept.Projects)
//                {
//                    Console.WriteLine($"  - {proj.Name} (Budget: ${proj.Budget:N0})");
//                }
//            }
//            else
//            {
//                Console.WriteLine("  No projects");
//            }
//            Console.WriteLine();
//        }
//    }

//    static void PerformComplexQueries(Collection<Department> departments)
//    {
//        Console.WriteLine("\nComplex Queries:");

//        // Find departments with projects having a budget over $100,000
//        var highBudgetDepts = departments.Find(d => d.Projects != null && d.Projects.Any(p => p.Budget > 100000));
//        Console.WriteLine($"Departments with high-budget projects: {string.Join(", ", highBudgetDepts.Select(d => d.Name))}");

//        // Find employees with C# skills across all departments
//        var csharpDevs = departments.Find(d => true)
//            .Where(d => d.Employees != null)
//            .SelectMany(d => d.Employees)
//            .Where(e => e.Skills != null && e.Skills.Contains("C#"))
//            .ToList();
//        Console.WriteLine($"C# developers: {string.Join(", ", csharpDevs.Select(e => e.Name))}");

//        // Find the department with the most employees
//        var largestDept = departments.OrderByDescending(d => d.Employees?.Count ?? 0).FirstOrDefault();
//        if (largestDept != null)
//        {
//            Console.WriteLine($"Largest department: {largestDept.Name} with {largestDept.Employees?.Count ?? 0} employees");
//        }
//        else
//        {
//            Console.WriteLine("No departments found");
//        }

//        // Calculate total budget across all projects
//        var totalBudget = departments.Find(d => true)
//            .Where(d => d.Projects != null)
//            .SelectMany(d => d.Projects)
//            .Sum(p => p.Budget);
//        Console.WriteLine($"Total budget across all projects: ${totalBudget:N0}");
//    }

//    static void UpdateNestedData(Collection<Department> departments)
//    {
//        Console.WriteLine("\nUpdating Nested Data:");

//        // Update a skill for an employee
//        var engineeringDept = departments.FirstOrDefault(d => d.Name == "Engineering");
//        if (engineeringDept != null && engineeringDept.Employees != null)
//        {
//            var alice = engineeringDept.Employees.FirstOrDefault(e => e.Name == "Alice Smith");
//            if (alice != null)
//            {
//                if (alice.Skills == null)
//                {
//                    alice.Skills = new List<string>();
//                }
//                alice.Skills.Add("React");
//                departments.Update(d => d.Id == engineeringDept.Id, engineeringDept);
//                Console.WriteLine("Added 'React' skill to Alice Smith.");
//            }
//        }

//        // Increase budget for a project
//        var marketingDept = departments.FirstOrDefault(d => d.Name == "Marketing");
//        if (marketingDept != null && marketingDept.Projects != null)
//        {
//            var brandCampaign = marketingDept.Projects.FirstOrDefault(p => p.Name == "Brand Campaign");
//            if (brandCampaign != null)
//            {
//                brandCampaign.Budget *= 1.1m; // 10% increase
//                departments.Update(d => d.Id == marketingDept.Id, marketingDept);
//                Console.WriteLine($"Increased budget for Brand Campaign to ${brandCampaign.Budget:N0}");
//            }
//        }
//    }

//    static void RemoveComplexData(Collection<Department> departments)
//    {
//        Console.WriteLine("\nRemoving Complex Data:");

//        // Remove all projects started more than a month ago
//        var oneMonthAgo = DateTime.Now.AddMonths(-1);
//        var allDepartments = departments.Find(d => true).ToList(); // Create a copy of the list
//        foreach (var dept in allDepartments)
//        {
//            if (dept.Projects != null)
//            {
//                var projectsToRemove = dept.Projects.Where(p => p.StartDate < oneMonthAgo).ToList();
//                foreach (var project in projectsToRemove)
//                {
//                    dept.Projects.Remove(project);
//                }
//                departments.Update(d => d.Id == dept.Id, dept);
//            }
//        }
//        Console.WriteLine("Removed all projects started more than a month ago.");

//        // Remove employees with no skills
//        allDepartments = departments.Find(d => true).ToList(); // Refresh the list
//        foreach (var dept in allDepartments)
//        {
//            if (dept.Employees != null)
//            {
//                var employeesToRemove = dept.Employees.Where(e => e.Skills == null || e.Skills.Count == 0).ToList();
//                foreach (var employee in employeesToRemove)
//                {
//                    dept.Employees.Remove(employee);
//                }
//                departments.Update(d => d.Id == dept.Id, dept);
//            }
//        }
//        Console.WriteLine("Removed all employees with no skills.");
//    }

//}

//public class Department
//{
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public Employee Manager { get; set; }
//    public List<Employee> Employees { get; set; }
//    public List<Project> Projects { get; set; }
//}

//public class Employee
//{
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public string Email { get; set; }
//    public List<string> Skills { get; set; }
//}

//public class Project
//{
//    public int Id { get; set; }
//    public string Name { get; set; }
//    public decimal Budget { get; set; }
//    public DateTime StartDate { get; set; }
//}
