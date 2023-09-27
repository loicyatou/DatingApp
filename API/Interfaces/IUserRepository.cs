﻿using API.Entities;

namespace API;

public interface IUserRepository
{
    void Update(AppUser user);

    Task<bool> SaveAllAsync();
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<IEnumerable<MemberDTO>> GetMembersAsync();
    Task<MemberDTO> GetMemberAsync(string username);
    Task<AppUser> GetUserByIDAsync(int id);
    Task<AppUser> GetUserByUsernameAsync(string username);

}
