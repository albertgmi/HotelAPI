﻿using FluentValidation;
using HotelAPI.Models.HotelModels;

namespace HotelAPI.Models.Validators.HotelValidators
{
    public class CreateHotelDtoValidator : AbstractValidator<CreateHotelDto>
    {
        public CreateHotelDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");
            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.");
            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Street is required.");
            RuleFor(x => x.ContactNumber)
                .NotEmpty().WithMessage("Contact number is required.")
                .Matches(@"^\d{3}-\d{3}-\d{3}$").WithMessage("Phone number must by in this pattern: ###-###-###.");
        }
    }
}
