using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")] //This defines the endpoint: specific location within an API that acepts requests and sends back responses. [controller] will use the first bit of the defined controller as the name. so to search this you will end the http with .../api/users
public class UsersController : ControllerBase
{

    private readonly DataContext _context; //Data Context: source of entities mapped to db. tracks changes made to all retrieve entries and maintains an identify cache so that multiple requests to the database dont make multiple instances and instea uses the existing one in the cache. Ensures you work with hte same entity across the entire applicatin context so that its consistent 

    public UsersController(DataContext context) //dependency injection. only need the datacontext in this specific controller. so it is intialised when this controller is called without the need to have it intialise in the entire code context. nicer way to maintain the code
    {
        _context = context;
    }

    //ActionResult: What a controller action returns in response to a browser request
    //IEnumerable: Used to iterate a given object in C#. Read-Only data strucutre s you cannot add or remove items from the list
    //Async: allows asynchronous programming i.e. when a method is declared aysnch and Task<> object used he code can be executed independently from the rest of the program. This allows other tasks or operations to continue executing while the asynchronous code is running. Once it reaches the await key word then it stops the main thread and waits to get the result of the aysnch method. Once the awaited task is complete, the execution of the method resumes from where it left off
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        return await _context.Users.ToListAsync(); 
    }

    [HttpGet("{id}")] //overloads method so that you can search for a specific user based off their ID. i.e. //api/users/2
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        return _context.Users.Find(id);
    }

}
