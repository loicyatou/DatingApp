using API.Entities;
using Microsoft.AspNetCore.Identity;

namespace API;

//IdentiyRole is an ASPNet class that contains information about user roles. This is for Role based authroisation. 

public class AppRole : IdentityRole<int>
{
    public ICollection<AppUserRole> UserRoles {get;set;}

}
