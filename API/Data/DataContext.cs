using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API;

//There is a specific order that the relationships and maping for the db context must be in when using IdentityDbContext this is
//IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>

public class DataContext : IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>,
AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>> //taken from nuget gallaery Microsoft.aspnet.identity.entiryframewoerk etc 
{

    // Represents the database context for the application, extending IdentityDbContext with custom user and role entities.

    // Generic Type Parameters:
    // - TUser: Represents the custom user entity type (AppUser).
    // - TRole: Represents the custom role entity type (AppRole).
    // - TKey: Specifies the type of the primary key for the user and role entities (int).
    // - TUserClaim: Represents additional claims associated with the user (IdentityUserClaim<int>).
    // - TUserRole: Represents the relationship between users and roles (AppUserRole).
    // - TUserLogin: Represents external logins for the user (IdentityUserLogin<int>).
    // - TRoleClaim: Represents additional claims associated with the role (IdentityRoleClaim<int>).
    // - TUserToken: Represents the authentication tokens for the user (IdentityUserToken<int>).

    public DataContext(DbContextOptions options) : base(options) //the base class is DbContext
    {
    }

    //IdentityDBContext does this for us dont need to declare this anymore
    // public DbSet<AppUser> Users { get; set; } 
    public DbSet<UserLike> Likes { get; set; }


    public DbSet<Message> Messages { get; set; }

    //creating a many to many relationship table manually etc
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>()
        .HasMany(ur => ur.UserRoles)
        .WithOne(u => u.User)
        .HasForeignKey(u => u.UserId)//identiftydbcontext sets this id
        .IsRequired();

        modelBuilder.Entity<AppRole>()
        .HasMany(ur => ur.UserRoles)
        .WithOne(u => u.Role)
        .HasForeignKey(u => u.RoleId)
        .IsRequired();

        //the primary key for this many to many relationship is the source and target id in terms of liking and reciecing likes
        modelBuilder.Entity<UserLike>()
        .HasKey(k => new { k.SourceUserId, k.TargetUserID });

        //foreign keys will come from the AppUser table
        modelBuilder.Entity<UserLike>()
        .HasOne(s => s.SourceUser) //s reps userlike entitiy and sourceuser reps navigation peroperty in userlike that reps the source user i.e the user who performed the liked action. a userlike entitity can only have one related appuser entity. there can only be one type of source user but that source user can like many accounts (below)
        .WithMany(l => l.LikedUsers) //app users can like many other users
        .HasForeignKey(s => s.SourceUserId)
        .OnDelete(DeleteBehavior.ClientCascade); //app user deletes then get of linked liked entities.

        modelBuilder.Entity<UserLike>()
        .HasOne(s => s.TargetUser)
        .WithMany(l => l.LikedByUsers)
        .HasForeignKey(s => s.TargetUserID)
        .OnDelete(DeleteBehavior.ClientCascade);

        //set up many to many relationship between appuser and messages
        modelBuilder.Entity<Message>()
            .HasOne(u => u.Recipient)//one receipent with...
            .WithMany(m => m.MessagesRecieved) //many messages recieved
            .OnDelete(DeleteBehavior.Restrict); //as long as both users have not deleted the record keep it on the record of the user who hasnt deleted it

        modelBuilder.Entity<Message>()
            .HasOne(u => u.Sender)//one receipent with...
            .WithMany(m => m.MessagesSent) //many messages recieved
            .OnDelete(DeleteBehavior.Restrict); //as long as both users have not deleted the record keep it on the record of the user who hasnt deleted it

    }

}
