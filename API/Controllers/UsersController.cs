
using System.Security.Claims;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize] //Attribute encoforces validation where all requests must be authencicated with an appropriate web token
public class UsersController : BaseApiController
{

    private readonly IUserRepository _userRepository;
    private readonly IMapper _IMapper;
    public UsersController(IUserRepository userRepository, IMapper imapper)
    {
        this._userRepository = userRepository;
        this._IMapper = imapper;
    }

    //ActionResult: What a controller action returns in response to a browser request
    //IEnumerable: Used to iterate a given object in C#. Read-Only data strucutre s you cannot add or remove items from the list
    //Async: allows asynchronous programming i.e. when a method is declared aysnch and Task<> object used he code can be executed independently from the rest of the program. This allows other tasks or operations to continue executing while the asynchronous code is running. Once it reaches the await key word then it stops the main thread and waits to get the result of the aysnch method. Once the awaited task is complete, the execution of the method resumes from where it left off

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
    {
        var users = await _userRepository.GetMembersAsync();

        return Ok(users); //returns a new usertable filling the memberDTO instead with vals from AppUser
    }

    [HttpGet("{username}")] //overloads method so that you can search for a specific user based off their ID. i.e. //api/users/2
    public async Task<ActionResult<MemberDTO>> GetUser(string username)
    {
        return await _userRepository.GetMemberAsync(username);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
    {

        //This is checking the claims of the user token and retreiving the value of the nameIdentifier of the user trying to make an update request. 
        var Username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //then it checks to see if that user exists in the database
        var user = await _userRepository.GetUserByUsernameAsync(Username);

        if (user == null) return NotFound();

        //maps the information thats updated to the user found in the database and stores the changes in the datacontext
        _IMapper.Map(memberUpdateDTO, user);

        //triggers the change in the database
        if (await _userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update the user");


    }

}
