using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppUser : IdentityUser<int> //part of .net --> creates superior hashing for you. the main properties for this are handles for you and you can call it by default. they are commented out below ==> <int> tells .net we want our appuser id to be an int
{

    // public int Id { get; set; }
    // public string UserName { get; set; }

    // public byte[] PasswordHash { get; set; }

    // public byte[] PasswordSalt { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public int Age { get; set; }

    public string KnownAs { get; set; }

    public DateTime Created { get; set; }

    public DateTime LastActive { get; set; }

    public string Gender { get; set; }
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public string City { get; set; }
    public string Country { get; set; }


    public List<Photo> Photos { get; set; } = new(); //Since the [table] attribute is passed to the Photo class a specific users photos are auto mapped to each other

    //two navigation properties
    //- This relationship indicates that an `AppUser` can have multiple `UserLike` entities in the `LikedByUsers` collection, representing the users who have liked them.
    //- And an `AppUser` can have multiple `UserLike` entities in the `LikedUsers` collection, representing the users they have liked.
    public List<UserLike> LikedByUsers { get; set; }
    public List<UserLike> LikedUsers { get; set; }


    //navigation properties for messages sent and recieved
    public List<Message> MessagesSent { get; set; }
    public List<Message> MessagesRecieved { get; set; }

    public ICollection<AppUserRole> UserRoles { get; set; }

}