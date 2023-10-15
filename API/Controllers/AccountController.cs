using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using API.Entities;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API;

public class AccountController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper _mapper)
    {
        this._userManager = userManager;
        this._tokenService = tokenService;
        this._mapper = _mapper;
    }

    //

    //Post Request: To create --> in this case it is to register a new user 
    [HttpPost("register")] //to access this endpoint the user has to access api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {

        if (await UserExists(registerDto.UserName)) { return BadRequest("Username is taken"); }

        //using: defines a scope at the end of which an object is disposed. Needed here becuase HMAC will use resources without destroying them at the end

        var user = _mapper.Map<AppUser>(registerDto);

        user.UserName = registerDto.UserName.ToLower();

        var result = await _userManager.CreateAsync(user, registerDto.Password); //creates the user into the new identity context and saves it automatically

        if(!result.Succeeded) return BadRequest(result.Errors); 

        var roleResult = await _userManager.AddToRoleAsync(user, "Member");

        if(!roleResult.Succeeded) return BadRequest(result.Errors);

        return new UserDto //When a user attempts to register, if all goes well the JSON response is the users username and token
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    //login a user 
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {

        var user = await _userManager.Users
        .Include(p => p.Photos)//data context doesnt auto cause relation between two tables so you must eagerly load the photos & appuser class together. i.e always include your relations so they are mapped togehther when your creating an instance of a table entity
        .SingleOrDefaultAsync(x => x.UserName.ToLower() == loginDto.UserName.ToLower()); //Singleor.. method will return the first row that matches the parameter passed. if it doesnt exist it will return the default of the object whichi in this case is null

        if (user == null) return Unauthorized("Invalid username"); //http response that action is not auth

        var result = await _userManager.CheckPasswordAsync(user,loginDto.Password); //identity now checks for us whether the password is true or false

        if(!result) return Unauthorized("Invalid Password");

        return new UserDto //When a user attempts to login, if all goes well the JSON response is the users username and token
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url, //find if they have a main photo and store it on local storage
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }


    //Determine whether a user exists within the database already before registering them
    private async Task<bool> UserExists(string username)
    {
        //it is going to check the db to see if the user exists. if it does it will return true and false otherwise. 

        //it will iterate through the table to check if there are any matching usernames
        return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
    }

}
