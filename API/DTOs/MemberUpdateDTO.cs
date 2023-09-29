namespace API;

//map changes made on the client to the AppUser so changes are made to the DB.
public class MemberUpdateDTO
{
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
}
