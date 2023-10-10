
using API.Entities;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace API;

public class LikesRepository : ILikesRepository
{
    private readonly DataContext _context;

    public LikesRepository(DataContext _context)
    {
        this._context = _context;
    }

    //get the userlike entity that matches the primary key
    public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await _context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    //return list of user likes based off the predicate --> if userId is source then you want list of users liked by the source or target which is list of users that user has liked
    public async Task<PagedList<LikedDTO>> GetUserLikes(LikesParams likesParams)
    {
        var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
        var likes = _context.Likes.AsQueryable();

        //show the list of users liked by the user provided
        if (likesParams.Predicate == "liked")
        {
            likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
            users = likes.Select(like => like.TargetUser);
        }

        if (likesParams.Predicate == "liked")
        {
            likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
            users = likes.Select(like => like.TargetUser);
        }

        var likedUsers = users.Select(user => new LikedDTO
        {
            UserName = user.UserName,
            KnownAs = user.KnownAs,
            Age = user.DateOfBirth.CalculateAge(),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
            City = user.City,
            Id = user.Id
        });

        return await PagedList<LikedDTO>.CreateAsync(likedUsers,likesParams.PageNumber,likesParams.PageSize);
    }

    public async Task<AppUser> GetUserWithLikes(int userId)
    {
        return await _context.Users
        .Include(x => x.LikedByUsers) //includ the related table in the query
        .FirstOrDefaultAsync(x => x.Id == userId);
    }
}
