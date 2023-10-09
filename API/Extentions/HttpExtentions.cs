using System.Text.Json;

namespace API;

public static class HttpExtentions
{
public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header){
    var jsonOptions = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase}; //adds some convention to the JSON like forcing the property names to be in Camelcase. This is so that it matchesjavascript naming conventions and makes it more readable

    response.Headers.Add("Pagination", JsonSerializer.Serialize(header,jsonOptions));

    response.Headers.Add("Access-Control-Expose-Headers", "Pagination"); //tells the serer this custom header is safe to expose to a different domain than the one the response came from. This is part of the cors safety medchanism which essnetialy protects users on the web from malicious cross-origin request.
    }
}
