using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API;

public class Seed
{
    public static async Task SeedUsers(DataContext context)
    {
        if (await context.Users.AnyAsync()) return; //checks if there are any users in the database already. If there is do not add new users since they are likely already present

        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json"); //Take the userData from the JSON file and creates an object full of data.

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true }; //incase the JSON file does not meet the scheme it will not error if some properties arent in uppercase for first letter

        var users = JsonSerializer.Deserialize<List<AppUser>>(userData); //deserializes the user data which was in filestream object format into a list of users in the AppUser format. 

        foreach (var user in users)
        {
            //creates a hashed password for each user
            using var hmac = new HMACSHA512();
            user.UserName = user.UserName.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
            user.PasswordSalt = hmac.Key;

            context.Users.Add(user); //tracks the users that are to be added to the database
        }

        await context.SaveChangesAsync(); //saves the new users to the database 
    }
}
