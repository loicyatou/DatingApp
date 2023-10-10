using Microsoft.AspNetCore.Mvc;

namespace API;

public class LikesController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly ILikesRepository _likesRepository;

    public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
    {
        _userRepository = userRepository;
        _likesRepository = likesRepository;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
        var sourceUserId = User.GetUserID(); //get user id of user signed in
        var likedUser = await _userRepository.GetUserByUsernameAsync(username); //find instance of user that matches the username provided
        var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId); //find instance of user that wants to like a users page in the likes table

        if (likedUser == null) return NotFound();

        if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

        var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id); //check to see if the userLikes table already contains this relationship

        if (userLike != null) return BadRequest("You already like this user"); //doesnt accept duplicates so the user has already liked this user

        //userlike should be empty if a like hasnt already been made so a new instance is created 
        userLike = new UserLike
        {
            SourceUserId = sourceUserId,
            TargetUserID = likedUser.Id
        };

        sourceUser.LikedUsers.Add(userLike);

        if (await _userRepository.SaveAllAsync()) return Ok();

        return BadRequest("Failed to like user");
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<LikedDTO>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {

        likesParams.UserId = User.GetUserID();

        var users = await _likesRepository.GetUserLikes(likesParams); //depending on predicate return the list of users liked by the user or list of users users has liked

        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

        return Ok(users);
    }
}
