namespace API.Entities;

public class MemberDTO
{
    public int Id { get; set; } //Id is a default term for the primary key but if you wanted a diff name then give it a [key] attribute
    public string UserName { get; set; }

    public int Age { get; set; }

    public string PhotoURL { get; set; }

    public string KnownAs { get; set; }

    public DateTime Created { get; set; }

    public DateTime LastActive { get; set; }

    public string Gender { get; set; }
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public string City { get; set; }
    public string Country { get; set; }

    public List<PhotoDTO> Photos { get; set; }
}