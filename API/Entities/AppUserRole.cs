using API.Entities;
using Microsoft.AspNetCore.Identity;

namespace API;

//represents the join table between appusers and role 
public class AppUserRole : IdentityUserRole<int>
{
    public AppUser User {get; set;}
    public AppRole Role {get; set;}
}
