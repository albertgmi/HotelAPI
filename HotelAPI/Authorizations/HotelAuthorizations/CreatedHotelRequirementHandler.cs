using HotelAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HotelAPI.Authorizations.HotelAuthorizations
{
    public class CreatedHotelRequirementHandler : AuthorizationHandler<CreatedHotelRequirement, Hotel>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CreatedHotelRequirement requirement, Hotel resource)
        {
            if (requirement.Operation == ResourceOperation.Read)
                context.Succeed(requirement);
            var userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            if (resource.CreatedById == userId)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
