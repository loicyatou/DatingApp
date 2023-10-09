using Microsoft.AspNetCore.Mvc.Filters;

namespace API;

//This is an Action Filter: An action filter is some logic that can execute before or after a controller action 
public class LogUserActivity : IAsyncActionFilter
{

    //ActionExecutingContext: class that reps the context of action being executed i.e. before the action method is invoked
    //ActionExecutionDelegate: reps the next step of the middleware pipieline. Typically used with middleware components to invoke the next middlewarein the pipeline.
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next(); //Allows next middelware component in pipeline and stores the result in the resultContext variable. THis allows the api action to be executed and get the result

        if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return; //check if the user is authenticated. This is just prepemtive it is unlikely that the user wont be authentiated since the controllers it connects to ant be used without it anyway

        var userId = resultContext.HttpContext.User.GetUserID();//get users username

        var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>(); //using dependency injection we get acess to the IUserRepository. This is done through dependancy injectio for its advantages

        var user = await repo.GetUserByIDAsync(int.Parse(userId)); //could use username but having iD makes query faster and sharper
        user.LastActive = DateTime.UtcNow; //whole purpose of method is to update the users Last Active property.
        await repo.SaveAllAsync();
    }
}
