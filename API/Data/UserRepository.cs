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

        public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
    {
        return await _context.Users
        .ProjectTo<MemberDTO>(_imapper.ConfigurationProvider)
        .ToListAsync();
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
