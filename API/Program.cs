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
app.UseCors( builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200")); //The API is on one server, angular on another so we use this so that the instance of the web app we use allow cross domain requests. brower requires us to declare this since i blocks domains outside of host address for security reasons

app.UseAuthentication(); //asks if you have a valid token
app.UseAuthorization(); //asks what your allowed to do since you have a valid token. order matters here 


app.MapControllers();

app.Run();
