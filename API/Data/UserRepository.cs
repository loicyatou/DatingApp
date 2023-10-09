using API.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly IMapper _imapper;

    public UserRepository(DataContext context, IMapper imapper)
    {
        _context = context;
        this._imapper = imapper;
    }

    public async Task<AppUser> GetUserByIDAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
        .Include(p => p.Photos) //so that it also includes related data in other tables foreign keys such as the photos table
        .SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<MemberDTO> GetMemberAsync(string username)
    {
        //rather than creating a query that searches every column of the db now it only looks at where the username matches the username provided in the method. Makes querying more efficient and less taxing on the DB
        return await _context.Users
        .Where(x => x.UserName.Equals(username))
        .ProjectTo<MemberDTO>(_imapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await _context.Users
        .Include(p => p.Photos) //Eaglery loading: so that it also includes related data in other tables foreign keys. You dont need to do this however with a automapper as you can see below
        .ToListAsync();
    }

    public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams)
    {
        var query = _context.Users.AsQueryable();

        //query defintion
        query = query.Where(u => u.UserName != userParams.CurrentUsername); //remove from set users who match current users signed in
        query = query.Where(u => u.Gender == userParams.Gender); //only add those users who are he opposite gender


        //age of matches 
        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));
        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);


        query = userParams.OrderBy switch //switch statement which determines the differnet conditions underwhich a filter will apply
        {
            "created" => query.OrderByDescending(u => u.Created), //if userParams.OrderBy is = "created@ it will order the list by the descending order they were createed
            _ => query.OrderByDescending(u => u.LastActive) //oitherwise it will default to the dates they were last active
        };

        //when a user makes a request for all the users it will now return those users into seperate pages depending on the page number and pagesize specified.
        return await PagedList<MemberDTO>.CreateAsync(
        query.ProjectTo<MemberDTO>(_imapper.ConfigurationProvider),
        userParams.PageNumber,
        userParams.PageSize);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public void Update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }
}
