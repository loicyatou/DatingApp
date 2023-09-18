using API;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt => 
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
}); //This method is informing the web instance where to get its connection to the database from ==

var app = builder.Build();

// Configure the HTTP request pipeline. 
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
