namespace Services.Validators.QueryValidators
{
    using Domain.Dtos;
    using FluentValidation;

    public class GetUsersByFilterQueryValidator : AbstractValidator<GetUsersByFilterQuery>
    {
        public GetUsersByFilterQueryValidator()
        {
            When(payload => payload.Age != null && payload.Country != null, () =>
            {
                RuleFor(payload => payload)
                    .Must(payload => false)
                    .WithMessage("Only 'Age' or 'Country' can be provided, not both.");
            });

            When(payload => payload.Age == null && payload.Country == null, () =>
            {
                RuleFor(payload => payload)
                    .Must(payload => false)
                    .WithMessage("'Age' or 'Country' are needed");
            });

            RuleFor(payload => payload.Age)
                .NotNull().When(payload => payload.Country == null)
                .GreaterThanOrEqualTo(1).When(payload => payload.Country == null);

            RuleFor(payload => payload.Country)
                .NotEmpty().When(payload => payload.Age == null);

            RuleFor(payload => payload.PageSize).GreaterThanOrEqualTo(1);
            RuleFor(payload => payload.PageNumber).GreaterThanOrEqualTo(1);
        }
    }
}
