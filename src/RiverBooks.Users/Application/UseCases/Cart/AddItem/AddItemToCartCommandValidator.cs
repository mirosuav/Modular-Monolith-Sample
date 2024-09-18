using FluentValidation;

namespace RiverBooks.Users.Application.UseCases.Cart.AddItem;

public class AddItemToCartCommandValidator : AbstractValidator<AddItemToCartCommand>
{
    public AddItemToCartCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Not a valid UserId.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be at least 1.");

        RuleFor(x => x.BookId)
            .NotEmpty()
            .WithMessage("Not a valid BookId.");
    }
}