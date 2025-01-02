using System.Security.Claims;

namespace HotelAPI.Services.UserServiceFolder
{
    public interface IUserContextService
    {
        ClaimsPrincipal User { get; }
        int? GetUserId { get; }
    }
}
