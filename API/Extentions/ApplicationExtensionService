using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API;

public static class ApplicationExtensiosService

//IServiceCollerction represents a container for registering and ersolving services that can be used throughout an application
{

    //By using the `this` keyword in the method signature, it gives the appearance that the method is part of the `IServiceCollection` class itself, even though it is actually defined as an extension method in a separate static class.

    //Extension methods allow you to extend existing classes without modifying their source code. They provide a way to add additional functionality to a class without having to inherit from it or create a separate wrapper class.
    
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)

    {
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        }); //This method is informing the web instance where to get its connection to the database from 

        services.AddCors();

        services.AddScoped<ITokenService, TokenService>(); //services can be given different life times (duration within which ann instance of a service is created and maintained) here addScoped means a new instance of the service is created for each sope or request. the instance is then available or the lifetime of that requuest. once the request is completed the instance is disposed off
        return services;

    }


}
