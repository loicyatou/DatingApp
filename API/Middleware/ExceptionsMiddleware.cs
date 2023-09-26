using System.Net;
using System.Text.Json;

namespace API;

public class ExceptionsMiddleware
{

    private readonly RequestDelegate _next; //reference to the next middleware component in the pipeline
    private readonly ILogger<ExceptionsMiddleware> _logger; //allows you to log information, warnngs, errors and other messagesS
    private readonly IHostEnvironment _env; //the hosting envrioment. provides info about which envrioment your in, whether thats developement, staging or production

    public ExceptionsMiddleware(RequestDelegate next, ILogger<ExceptionsMiddleware> logger,
     IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); //calls the next middleware component in the pipeline. It allows the request to continue its normal execution. if an exception occurs in the middleware comp it called it gets caught below
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);//logs the specifici exception in the log instance

            //sets up the error response to be returned to the client

            context.Response.ContentType = "application/json"; //sets the content type to JSON as opposed to html
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; //sets a 500 status code: means an unexpeced error that prevents the serve from fulfilling the request made by the client. serialises 

            var response = _env.IsDevelopment() ? new APIException(context.Response.StatusCode, ex.Message, ex.StackTrace.ToString())
            : new APIException(context.Response.StatusCode, ex.Message, "Internal Server Error"); //only exposes details about the error in the development enviroment to assist dev but not in production as it can cause informaton leakage about sensitive info concerning the system

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }; //this instance will confgre the JSON serialization process. Here it states that the property names in the serialized JSON shhould be converted to camcel case

            var json = JsonSerializer.Serialize(response, options); //serializes the response object which here is an instance of the API exception class we madeinto a JSON string 

            await context.Response.WriteAsync(json); //sends the response to the response stream which will be sent back to the client in an error response
        }
    }




}
