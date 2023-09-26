using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using API.Entities;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API;

public class AccountController : BaseApiController
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    public AccountController(DataContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    //

    //Post Request: To create --> in this case it is to register a new user 
    [HttpPost("register")] //to access this endpoint the user has to access api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {

        if (await UserExists(registerDto.UserName)) { return BadRequest("Username is taken"); }

        //using: defines a scope at the end of which an object is disposed. Needed here becuase HMAC will use resources without destroying them at the end

        using var hmac = new HMACSHA512(); //randomly generated key that we are going to use as the password salt

        var user = new AppUser
        {
            UserName = registerDto.UserName.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)), //Encoding: put seq of characters into a speciaised format for effiecnt transmision or storage. this is so that you can put the password into the byte array. 
            PasswordSalt = hmac.Key
        };

        _context.Users.Add(user); //does not auto add users into DB yet but stores them in memory ready to perform a DB operation once you request a sync. 

        await _context.SaveChangesAsync(); //tracks for any changes made in the dataBase context ^ and performs the update. 

        return new UserDto //When a user attempts to register, if all goes well the JSON response is the users username and token
        {
            UserName = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }

    //login a user 
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        
        var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName.ToLower() == loginDto.UserName.ToLower()); //Singleor.. method will return the first row that matches the parameter passed. if it doesnt exist it will return the default of the object whichi in this case is null

        if (user == null) return Unauthorized("Invalid username"); //http response that action is not auth

        using var hmac = new HMACSHA512(user.PasswordSalt); //1. get the users salt val associated with account

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password)); //2. get what the user has inputted as a password

        //3. use a for loop and iterate through each character of the byte array^ and see if there are in   consistencies betwee the hashed password version theyve inputted & whats available in the database 

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i])
            {
                return Unauthorized("Invalid Password");
            }
        }


        return new UserDto //When a user attempts to login, if all goes well the JSON response is the users username and token
        {
            UserName = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }


    //Determine whether a user exists within the database already before registering them
    private async Task<bool> UserExists(string username)
    {
        //it is going to check the db to see if the user exists. if it does it will return true and false otherwise. 

        //it will iterate through the table to check if there are any matching usernames
        return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }

}
