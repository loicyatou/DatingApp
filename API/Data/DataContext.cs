using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API;

public class DataContext : DbContext
{

    public DataContext(DbContextOptions options) : base(options) //the base class is DbContext
    {
    }

    public DbSet<AppUser> Users { get; set; } //DbSet is a table so here the AppUser class is the table and its variables the columns
    public DbSet<UserLike> Likes { get; set; }

    //creating a many to many relationship table manually so that users can like many profiles and also have there profiled liked by many users
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
    }

}
