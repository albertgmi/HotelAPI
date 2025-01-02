using Microsoft.AspNetCore.Authorization;

namespace HotelAPI.Authorizations.ReservationAuthorizations
{
    public class CreatedReservationRequirement : IAuthorizationRequirement
    {
        public ResourceOperation Operation { get; set; }
        public CreatedReservationRequirement(ResourceOperation operation)
        {
            Operation = operation;
        }
    }
}
