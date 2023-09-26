using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API;

public class BuggyController : BaseApiController
{

    private readonly DataContext _context;
    public BuggyController(DataContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetSecret()
    {
        return "secret text";
    }


    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFOund()
    {
        var thing = _context.Users.Find(-1); //not poss to find a user with this number

        if (thing == null) return NotFound();
        return thing; //will always return the error
    }


    [HttpGet("server-error")]
    public ActionResult<string> GetServerError()
    {
        var thing = _context.Users.Find(-1); //not poss to find a user with this number

        var thingToReturn = thing.ToString();

        return thingToReturn;
    }


    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("This was not a good request!");
    }
}
