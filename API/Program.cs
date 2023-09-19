using API;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args); //instance of the web app

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt => 
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
}); //This method is informing the web instance where to get its connection to the database from 

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline. 
//!The order of configurations here is very important. 
app.UseCors( builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200")); //The API is on one server, angular on another so we use this so that the instance of the web app we use allow cross domain requests. brower requires us to declare this since i blocks domains outside of host address for security reasons

app.MapControllers();

app.Run();
