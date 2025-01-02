using HotelAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HotelAPI.Authorizations.ReservationAuthorizations
{
    public class CreatedReservationRequirementHandler : AuthorizationHandler<CreatedReservationRequirement, Reservation>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CreatedReservationRequirement requirement, Reservation resource)
        {
            if (requirement.Operation == ResourceOperation.Create || requirement.Operation == ResourceOperation.Read)
                context.Succeed(requirement);
            var userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            if (resource.MadeById == userId)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
