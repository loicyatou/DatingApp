using API.Entities;

namespace API;

public interface ILikesRepository
{

    //method: get the userlike entity that belongs to a unique primary key of source to target user
    Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);

    //predicate: do they want to get the users theyve liked or are liked by. will be used for both so some logic will look at predicte and calc what is needed to be returned 
    Task<PagedList<LikedDTO>> GetUserLikes(LikesParams likesParams);

    //get the list of likes by a specific user
    Task<AppUser> GetUserWithLikes(int userId);

}
