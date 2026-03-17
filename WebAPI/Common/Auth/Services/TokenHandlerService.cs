using General.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Common.Auth.Services;

public class TokenRequirement : IAuthorizationRequirement;

public class TokenHandlerService<TContext>(TContext db, ClaimService claim)
    : AuthorizationHandler<TokenRequirement>
    where TContext : AppDbContext
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TokenRequirement requirement)
    {
        if (await db.Users.AnyAsync(x => x.Username == claim.Name))
            context.Succeed(requirement);

        else
            context.Fail();
    }
}