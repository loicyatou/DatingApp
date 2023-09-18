using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API;

public class DataContext : DbContext
{

    public DataContext(DbContextOptions options) : base(options) //the base class is DbContext
    {
    }
    
    public DbSet<AppUser> Users{get; set;} //DbSet is a table so here the AppUser class is the table and its variables the columns
    }
