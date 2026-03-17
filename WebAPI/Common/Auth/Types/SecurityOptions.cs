namespace WebAPI.Common.Auth.Types;

public class SecurityOptions
{
    public required string JwtKey { get; set; }
    
    public required string JwtAudience { get; set; }
        
    public required string JwtIssuer { get; set; }
    
    public required string[] AllowedOrigin { get; set; }
}