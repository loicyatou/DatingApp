using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API;

public class AdminController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;

    public AdminController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

//Policies allow you to define custom rules or requirements that must be met in order to access certain resources or perform specific actions.

[Authorize(Policy = "RequireAdminRole")] //users need this policy satisfiedot access this endpoint. this polciy is defined in the identify service extention
[HttpGet("users-with-roles")]
public async Task<ActionResult> GetUsersWithRoles()
{
    var users = await  _userManager.Users
    .OrderBy(u => u.UserName)
    .Select(u => new
    {
        u.Id,
        Username = u.UserName,
        Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
    })
    .ToListAsync();

    return Ok(users);
}

[Authorize(Policy = "RequireAdminRole")]
[HttpPost("edit-roles/{username}")]
public async Task<ActionResult> EditRoles(string username, [FromQuery]string roles){
   
    if(string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role"); //check if a role has been selected. atleast one role must be assigned by the admin

    var selectedRoles = roles.Split(",").ToArray(); //can have multiple roles

    var user = await _userManager.FindByNameAsync(username); //find the user whos roles are being changed

    if (user ==null) return NotFound();

    var userRoles = await _userManager.GetRolesAsync(user);//get the users current roles

    var result = await _userManager.AddToRolesAsync(user,selectedRoles.Except(userRoles)); //add the new roles to that user except the roles they originally had

    if(!result.Succeeded) return BadRequest("Failed to add to roles");

    result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles)); //remove there old roles except the new ones added

    if(!result.Succeeded) return BadRequest("Failed to remove from roles");

    return Ok(await _userManager.GetRolesAsync(user)); //return the adjusted roles in an ok response

}



[Authorize(Policy = "ModeratePhotoRole")]
[HttpGet("photos-to-moderate")]
public ActionResult GetPhotosForModeration()
{
    return Ok("Only admins or moderators can see this");
}


}
