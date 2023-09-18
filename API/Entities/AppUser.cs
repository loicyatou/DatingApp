namespace API.Entities;

public class AppUser
{
    public int Id { get; set; } //Id is a default term for the primary key but if you wanted a diff name then give it a [key] attribute
    public string UserName {get; set;} = "No Username";

    



}
