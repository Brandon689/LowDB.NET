using System;
using System.Linq;
using LowDBNet;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("LowDB.NET Demo");
        Console.WriteLine("==============");

        // Initialize the database (this will use file storage if possible, otherwise in-memory)
        var db = new LowDB("testdb.json");

        // Get a collection
        var users = db.GetCollection<User>("users");

        // Insert some data (this will automatically write to storage)
        users.Insert(new User { Id = 1, Name = "Alice", Age = 30 });
        users.Insert(new User { Id = 2, Name = "Bob", Age = 25 });
        users.Insert(new User { Id = 3, Name = "Charlie", Age = 35 });

        Console.WriteLine("Inserted 3 users.");

        // Find and display all users
        Console.WriteLine("\nAll users:");
        foreach (var user in users.Find(u => true))
        {
            Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}");
        }

        // Update a user (this will automatically write to storage)
        users.Update(u => u.Id == 2, new User { Id = 2, Name = "Bob", Age = 26 });
        Console.WriteLine("\nUpdated Bob's age to 26.");

        // Find and display a specific user using FirstOrDefault
        var bob = users.FirstOrDefault(u => u.Name == "Bob");
        Console.WriteLine($"\nBob's details: Id: {bob.Id}, Name: {bob.Name}, Age: {bob.Age}");

        // Remove a user (this will automatically write to storage)
        users.Remove(u => u.Id == 3);
        Console.WriteLine("\nRemoved Charlie from the database.");

        // Display all users again, ordered by age
        Console.WriteLine("\nRemaining users (ordered by age):");
        foreach (var user in users.OrderBy(u => u.Age))
        {
            Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}");
        }

        // Demonstrate Take method
        Console.WriteLine("\nFirst user:");
        var firstUser = users.Take(1).FirstOrDefault();
        Console.WriteLine($"Id: {firstUser.Id}, Name: {firstUser.Name}, Age: {firstUser.Age}");

        // Demonstrate OrderByDescending
        Console.WriteLine("\nUsers ordered by name descending:");
        foreach (var user in users.OrderByDescending(u => u.Name))
        {
            Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}");
        }

        Console.WriteLine("\nAll operations have been automatically saved to the database.");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}

// User class for our example
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}
