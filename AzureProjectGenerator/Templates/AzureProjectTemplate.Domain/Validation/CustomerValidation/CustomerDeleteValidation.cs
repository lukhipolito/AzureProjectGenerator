using FluentValidation;
using AzureProjectTemplate.Domain.Models;

namespace AzureProjectTemplate.Domain.Validation.CustomerValidation
{
    public class CustomerDeleteValidation : AbstractValidator<Customer>
    {
        public CustomerDeleteValidation()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .WithMessage("Id não pode ser nulo");
        }
    }
}
