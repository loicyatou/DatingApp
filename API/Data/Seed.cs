using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace API;

// Overall, this method reads user data from a JSON file, converts it into `AppUser` objects, and uses the `UserManager` to create these users in the database. It's commonly used for seeding initial user data during application startup or database initialization. 

public class Seed
{
    public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        if (await userManager.Users.AnyAsync()) return; //checks if there are any users in the database already. If there is do not add new users since they are likely already present

        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json"); //Take the userData from the JSON file and creates an object full of data.

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true }; //incase the JSON file does not meet the scheme it will not error if some properties arent in uppercase for first letter

        var users = JsonSerializer.Deserialize<List<AppUser>>(userData); //deserializes the user data which was in filestream object format into a list of users in the AppUser format. 

        var roles = new List<AppRole> //creates a list of roles to add to the DB
        {
            new AppRole{Name = "Member"},
            new AppRole{Name = "Admin"},
            new AppRole{Name = "Moderator"}
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role); //stores each of the roles created above into the database. These roles can then be assaigned to the users later on
        }

        foreach (var user in users)
        {
            user.UserName = user.UserName.ToLower();
            await userManager.CreateAsync(user, "Pa$$w0rd");
            await userManager.AddToRoleAsync(user, "Member"); //each new user created is automatically a member role
        }

        //creates an admin user. but remember that this data seed is only injected once in the database. so only one will be created whilst there can be numerous users added. 
        var admin = new AppUser
        {
            UserName = "admin"
        };

        await userManager.CreateAsync(admin, "Pa$$w0rd");
        await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" }); //gives it multiple roles

        // await context.SaveChangesAsync(); //saves the new users to the database 
    }
}
