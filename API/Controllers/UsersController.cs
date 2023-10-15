
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
    private readonly IPhotoService _photoService;

    public UsersController(IUserRepository userRepository, IMapper imapper,
    IPhotoService photoService)
    {
        this._userRepository = userRepository;
        this._IMapper = imapper;
        _photoService = photoService;
    }

    //ActionResult: What a controller action returns in response to a browser request
    //IEnumerable: Used to iterate a given object in C#. Read-Only data strucutre s you cannot add or remove items from the list
    //Async: allows asynchronous programming i.e. when a method is declared aysnch and Task<> object used he code can be executed independently from the rest of the program. This allows other tasks or operations to continue executing while the asynchronous code is running. Once it reaches the await key word then it stops the main thread and waits to get the result of the aysnch method. Once the awaited task is complete, the execution of the method resumes from where it left off

    [HttpGet]
    //[FromQuery]: gives a hint about where to find the query string parameters which in this case is from UserPramas so that when this method called the GetMembersAsync that method knows that the userParams is the query
    public async Task<ActionResult<PagedList<MemberDTO>>> GetUsers([FromQuery] UserParams userParams)
    {
        var currentUser = await _userRepository.GetUserByUsernameAsync(User.GetUsername()); //Usder is a claimsPrinciple provided by aspnet which essnetially knows info about the user that sent the request based off the authentication token. so it reads the users username, password and roles etc. so now you can get the user currently signed ins username so that you can remove them off the list of matches returned 
        userParams.CurrentUsername = currentUser.UserName;

        if (string.IsNullOrEmpty(userParams.Gender))
        { //if the pref gender of the users matches are unspecified..
            userParams.Gender = currentUser.Gender == "male" ? "female" : "male"; //...return matches of opposite gender
        }


        var users = await _userRepository.GetMembersAsync(userParams);


        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize,
        users.TotalCount, users.TotalPages)); //access to those methods since GetMemberAsync rertuns pagedlist.

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
        var Username = User.GetUsername();

        //then it checks to see if that user exists in the database
        var user = await _userRepository.GetUserByUsernameAsync(Username);

        if (user == null) return NotFound();

        //maps the information thats updated to the user found in the database and stores the changes in the datacontext
        _IMapper.Map(memberUpdateDTO, user);

        //triggers the change in the database
        if (await _userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update the user");


    }

    [HttpPost("add-Photo")]
    public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
    {
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return NotFound();

        var result = await _photoService.AddPhotoAsync(file);

        if (result.Error != null) return BadRequest(result.Error.Message); //checks if the error object of the result is null or not

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.Photos.Count == 0) photo.IsMain = true; //if it is there first photo upload then we set it to there main photo

        user.Photos.Add(photo); //adds the photo to the photo array in the datacontext


        if (await _userRepository.SaveAllAsync())
        {
            //sends back a 201 rathr than 200 so that th user gets more info/meta data about resource
            return CreatedAtAction(nameof(GetUser), new { username = user.UserName }, _IMapper.Map<PhotoDTO>(photo));
        }

        return BadRequest("Problem adding photo");

    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return NotFound();

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null) return NotFound();

        if (photo.IsMain) return BadRequest("this is already your main photo");

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain); //check what the current main photo is
        if (currentMain != null) currentMain.IsMain = false; //if there is a photo already on there set it to false

        photo.IsMain = true; //set new photo to true

        if (await _userRepository.SaveAllAsync()) return NoContent(); //sav changes to data context. i.e. save the new photo as the true new main pic

        return BadRequest("Problem setting the main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null) return BadRequest("You cannot delete your main photo");

        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await _userRepository.SaveAllAsync()) return Ok();

        return BadRequest("Problem deleting photo");
    }

}
