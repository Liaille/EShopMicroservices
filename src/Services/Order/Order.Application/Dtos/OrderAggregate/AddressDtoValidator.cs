using FluentValidation;

namespace Order.Application.Dtos.OrderAggregate;

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(address => address.FirstName)
            .NotEmpty().WithMessage("First name cannot be empty.");

        RuleFor(address => address.LastName)
            .NotEmpty().WithMessage("Last name cannot be empty.");

        RuleFor(address => address.Country)
            .NotEmpty().WithMessage("Country cannot be empty.");

        RuleFor(address => address.State)
            .NotEmpty().WithMessage("State cannot be empty.");

        RuleFor(address => address.AddressLine)
            .NotEmpty().WithMessage("Address line cannot be empty.");

        RuleFor(address => address.ZipCode)
            .NotEmpty()
            .Length(2, 10)
            .WithMessage("The zip code must be between 2 to 10 digits. Please check the input.");
        
        RuleFor(address => address.Email)
            .EmailAddress()
            .When(address => !string.IsNullOrEmpty(address.Email))
            .WithMessage("Email format error, please enter a valid email address (such as: xxx@xxx.com)");
    }
}
