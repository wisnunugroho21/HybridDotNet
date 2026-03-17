using System.IdentityModel.Tokens.Jwt;
using System.Text;
using General.Data;
using General.Data.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Common.API.Result;
using WebAPI.Common.Auth.Types;

namespace WebAPI.Common.Auth.Services;

public class AuthService(IOptions<SecurityOptions> options, AppDbContext db)
{
    public async Task<AuthResult> Login(string username, string password, CancellationToken cancellationToken)
    {
        var user = await db.Users.SingleOrDefaultAsync(x => x.Username == username, cancellationToken);
        
        if (user == null)
            return new AuthResult(false,string.Empty, ["username or password is incorrect"]);

        var pwd = new PasswordHasher<User>();
        var passwordVerificationResult = pwd.VerifyHashedPassword(user, user.Password, password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
            return new AuthResult(false,string.Empty, ["username or password is incorrect"]);

        var token = GenerateJwtToken(user);
        return new AuthResult(true, token, null);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(options.Value.JwtKey);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = options.Value.JwtIssuer,
            Audience = options.Value.JwtAudience,
            Claims = new Dictionary<string, object>() {
                { JwtRegisteredClaimNames.Name, user?.Username ?? "" },
                { JwtRegisteredClaimNames.Sub, user?.Username ?? "" }
            }
        };

        var token = jwtTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        var jws = jwtTokenHandler.WriteToken(token);

        return jws;
    }
}