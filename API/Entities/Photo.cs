using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Photos")] //This annotation will create a new table in our database and establish the foreign key relationship between the AppUser and the Photos table for you
    public class Photo
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; }

        public bool IsMain { get; set; }

        public int PublicId { get; set; }

        public int AppUserId {get; set;} //added this as per convention so that the no photos can be uplaoded that are not matched with an AppUser ID

        public AppUser AppUser {get; set;}
    }
}