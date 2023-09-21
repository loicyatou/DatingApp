using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {

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


        return services;
    }


}
