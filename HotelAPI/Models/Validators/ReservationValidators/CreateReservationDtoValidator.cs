using FluentValidation;
using HotelAPI.Models.ReservationModels;

namespace HotelAPI.Models.Validators.ReservationValidators
{
    public class CreateReservationDtoValidator : AbstractValidator<CreateReservationDto>
    {
        public CreateReservationDtoValidator()
        {
            RuleFor(x => x.CheckInDate)
                .NotEmpty().WithMessage("Check in date is required.")
                .Must(r => r >= DateTime.Now)
                .WithMessage("Date can't be from the past.");
            RuleFor(x => x.CheckOutDate)
                .NotEmpty().WithMessage("Check out date is required.");
        }
    }
}
