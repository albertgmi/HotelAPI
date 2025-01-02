using FluentValidation;
using HotelAPI.Models.RoomModels;

namespace HotelAPI.Models.Validators.RoomValidators
{
    public class UpdateRoomDtoValidator : AbstractValidator<UpdateRoomDto>
    {
        private readonly string[] roomTypes = new string[]
        {
            "Single", "Double", "Twin", "Triple", "Quad", "Suite",
            "King", "Queen", "Family", "Connecting", "Accessible",
            "Penthouse", "Dormitory"
        };
        public UpdateRoomDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MinimumLength(30).WithMessage("Description must be atleast 30 characters long");

            RuleFor(room => room.Type)
                .NotEmpty().WithMessage("Room type is required.")
                .Must(type => roomTypes.Contains(type))
                .WithMessage("Room type must be one of the following: " + string.Join(", ", roomTypes));

            RuleFor(x => x.PricePerNight)
                .GreaterThan(0).WithMessage("Price per night must be greater than 0.");
        }
    }
}
