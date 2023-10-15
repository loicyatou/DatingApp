using System.Text;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {

        //configurations for the identity services in the application (to do with the roles)
        services.AddIdentityCore<AppUser>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = false;
        }) //this configures the core diednetity service for the app. It specifies the user ideity type and allows you tocusstomise the password requirmetns for user accounts. Here we have said that user passwords do not require to contain non-alphanumeric characters. There are other options and default criterias provided by .net

        .AddRoles<AppRole>() //adding custom roles on top of the default roles provided by Microsoft. By default microsoft provide administrator and user with predefuined permissions and can be used for basic auhtorissation
        .AddRoleManager<RoleManager<AppRole>>()
        .AddEntityFrameworkStores<DataContext>(); //says that that this is the place to store uyser and role information 

        //JWTbearers is an authentication scheme by microsoft downloaded in the nuget to authenticate JWT tokens.
        //This service is customising the behaviour of the JWT bearer authneticatition middle wear
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, //that the token should be vaidated for a valid signing key 

                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])), //specifies the key to bee used for validation. whih was set in the config

                ValidateIssuer = false, //the issuer who who issues the key should not be validated as it was not accounted for when building the jwt token. i.e. is the issuer a trusted issuer pre-defined

                ValidateAudience = false //audience is intended recipient. i.e. does the reciever match the exepted value or is on the list of trusted audiences. again not accounted for so false
            };
        });

        //polcies that define what a user is able to do within an endpoint or not if these polcies are attahced to a controller then the user must satisfy the conditions defined within this in order ot access the information within it
        services.AddAuthorization(opt =>
        {
            opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
        });

        return services;
    }


}
