using FluentValidation;
using HotelAPI.Entities;
using HotelAPI.Models.RoomModels;
using Microsoft.IdentityModel.Tokens;

namespace HotelAPI.Models.Validators.RoomValidators
{
    public class RoomQueryValidator : AbstractValidator<RoomQuery>
    {
        private readonly int[] pageSize = new int[] { 5, 10, 15 };
        private readonly string[] allowedSortByColumnNames = new string[]
        {
            nameof(Room.Name), nameof(Room.Description),
            nameof(Room.Capacity), nameof(Room.PricePerNight),
            nameof(Room.IsAvailable)
        };
        public RoomQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize)
                .NotEmpty()
                .Must(pn => pageSize.Contains(pn));
            RuleFor(x => x.SortBy)
                .Must(value => value.IsNullOrEmpty() || allowedSortByColumnNames.Contains(value))
                .WithMessage($"Sort by is optional or must be in [{string.Join(", ", allowedSortByColumnNames)}]");
        }
    }
}
