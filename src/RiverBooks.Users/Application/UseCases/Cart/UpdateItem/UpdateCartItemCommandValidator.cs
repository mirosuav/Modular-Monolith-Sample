using FluentValidation;
using RiverBooks.Users.Application.UseCases.Cart.AddItem;

namespace RiverBooks.Users.Application.UseCases.Cart.UpdateItem;

public class UpdateCartItemCommandValidator : AbstractValidator<UpdateCartItemCommand>
{
    public UpdateCartItemCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Not a valid UserId.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantity must be greater or equal to zero.");
    }
}