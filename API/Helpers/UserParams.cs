namespace API;

//this class will set the defaults for the page numer and size that the user can adjust to an extent
public class UserParams : PaginationParams
{
    public string CurrentUsername { get; set; }
    public string Gender { get; set; }

    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 100;

    public string OrderBy { get; set; } = "lastActive";

}
