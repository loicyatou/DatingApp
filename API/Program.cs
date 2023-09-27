using System.Text;
using API;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args); //instance of the web app

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration); //IServiceContainer exteneded check meth
builder.Services.AddIdentityServices(builder.Configuration); //IServiceContainer exteneded check meth

//middleware: component that sits between the web server and the applications processing logic. it intercepts incoming requests and performs some processing and optionally parses the request ot the next middleware componetn in the pipeline which is why the order really matters.

var app = builder.Build(); //turning your icollection into a service provider i.e. creating instance of your service. 

// Configure the HTTP request pipeline. 
//!The order of configurations here is very important. 

app.UseMiddleware<ExceptionsMiddleware>();
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200")); //The API is on one server, angular on another so we use this so that the instance of the web app we use allow cross domain requests. brower requires us to declare this since i blocks domains outside of host address for security reasons

app.UseAuthentication(); //asks if you have a valid token
app.UseAuthorization(); //asks what your allowed to do since you have a valid token. order matters here 


app.MapControllers();


//! This block of code is adding our migrations and updating the database automatically as opposed to us using dotnet cli i.e. dotnet ef migrations etc to make changes to the database. It creates a scope for application services, retrieves the `DataContext` from the dependency injection container, applies pending database migrations, and seeds initial data into the database. Any errors that occur during these operations are logged using a logger
using var scope = app.Services.CreateScope(); //grabs the services currently given to the app instance and groups them into an object
var services = scope.ServiceProvider; //responsible for resolving and providing instances of services registered in the application's dependency injection container.
try 
{
    var context = services.GetRequiredService<DataContext>(); //grabs an instance of the datacontext. if it it cannot getRequiredService will throw exception

    await context.Database.MigrateAsync(); //applies any pending migraitons if it doesnt already exist in DB

    await Seed.SeedUsers(context); //inserts dummy data into the DB
}
catch (Exception ex) //have to use try catch because it wont pass through pipeline so wont get caught 
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");
}

app.Run();
