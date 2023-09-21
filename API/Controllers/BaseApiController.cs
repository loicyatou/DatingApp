using Microsoft.AspNetCore.Mvc;

namespace API;

[ApiController]
[Route("api/[controller]")] //This defines the endpoint: specific location within an API that acepts requests and sends back responses. [controller] will use the first bit of the defined controller as the name. so to search this you will end the http with .../api/users

public class BaseApiController : ControllerBase
{


}
