using System.IdentityModel.Tokens.Jwt;

namespace WebAPI.Common.Auth.Services;

public class ClaimService(IHttpContextAccessor accessor)
{
    public string Name => accessor.HttpContext?.User?.Claims?.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)?.Value.ToString()
                          ?? "";
    
    public string Sub => accessor.HttpContext?.User?.Claims?.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value.ToString()
                          ?? "";
}