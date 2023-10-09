using System.Security.Claims;

namespace API;

public static class ClaimsPrinciplesExtension 
{
public static string GetUsername(this ClaimsPrincipal User){
    return User.FindFirst(ClaimTypes.Name)?.Value;
}

public static string GetUserID(this ClaimsPrincipal User){
    return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}
}
