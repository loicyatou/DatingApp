namespace API;

//this class will set the defaults for the page numer and size that the user can adjust to an extent
public class UserParams
{
private const int MaxPageSize = 50;
public int PageNumber {get; set;}

private int _pageSize = 10;
public string CurrentUsername{get;set;}
public string Gender{get;set;}

//This code ensures that the `PageSize` property cannot be set to a value greater than `MaxPageSize`. If a value greater than `MaxPageSize` is assigned, it will automatically be capped at `MaxPageSize`.  By setting a maximum page size, you can prevent excessive resource consumption and optimize performance
public int PageSize{
    get => _pageSize;
    set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
}

public int MinAge{get; set;} = 18;
public int MaxAge{get; set;} = 100;

public string OrderBy{get;set;}= "lastActive";

}
