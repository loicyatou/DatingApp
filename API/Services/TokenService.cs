using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService : ITokenService
{

    //Symmetric Keys means that the key is used for both validating and generatic tokens. This is typicallu done when th token issuer and token consumer share the same secret key. 

    //Aysymmetric keys on the other hand is when the key for generating and validating keys are different. The public key would be the one for generating the key but the private for validating it. the public is shared but the private is key is kept a seceret

    private readonly SymmetricSecurityKey _key;
    private readonly UserManager<AppUser> _userManager;
    public TokenService(IConfiguration config, UserManager<AppUser> userManager)
    {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])); //config is instance of IConfiguation which represents the configuration settings for the application. What this does is it retrieves the value of the "tokenkey" configration setting. this configurtion file in real apps will be more secure but for this our config is the development.json file
        _userManager = userManager;
    }


    public async Task<string> CreateToken(AppUser user) //usermaner is part of identity
    {
        var claims = new List<Claim>
        {

            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName,user.UserName) //what will be in the claims about the token i.e. who the user claims to be. here it will be the users username
            
        };

        var roles = await _userManager.GetRolesAsync(user); //gets a list of roles the specifed user belongs to

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role))); //incase the user belongs to multiple roles you add a range to the end of the claims list. that way if theres only one thats fine and if theres more thats fine also

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature); //takes the key from config and decides the algo that will be used to generate a digital signature. the same algo will vadlidate the integirty of the signature during a session

        var tokenDescriptor = new SecurityTokenDescriptor //used to configure the properties of the token that will be generated.
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler(); //provided by the nuget open gallary. It is designed for creating and validating JSon web tokens

        var token = tokenHandler.CreateToken(tokenDescriptor); //pass the token handler the token details we want within the token

        return tokenHandler.WriteToken(token); //serializes the token into JWT format to be shared across the web
    }
}
