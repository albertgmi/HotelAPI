using FluentValidation;
using HotelAPI.Entities;
using HotelAPI.Models.HotelModels;
using Microsoft.IdentityModel.Tokens;

namespace HotelAPI.Models.Validators.HotelValidators
{
    public class HotelQueryValidator : AbstractValidator<HotelQuery>
    {
        private readonly int[] pageSize = new int[] { 5, 10, 15 };
        private readonly string[] allowedSortByColumnNames = new string[]
        {
            nameof(Hotel.Name), nameof(Hotel.Description),
            nameof(Hotel.Rating), nameof(Hotel.NumberOfRatings)
        };
        public HotelQueryValidator()
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
