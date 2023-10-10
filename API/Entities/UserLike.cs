using API.Entities;

namespace API;

//table that keeps track of the many to any relationship between users concerning who has liked what
public class UserLike
{
public AppUser SourceUser{get; set;}
public int SourceUserId{get; set;}
public AppUser TargetUser{get; set;}
public int TargetUserID{get; set;}

}
