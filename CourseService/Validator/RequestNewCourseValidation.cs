using CourseService.Dtos;
using FluentValidation;

namespace CourseService.Validator
{
    public class RequestNewCourseValidation:AbstractValidator<RequestNewCourse>
    {
        public RequestNewCourseValidation()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Course name is required.")
               .MaximumLength(100).WithMessage("Course name can't exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description can't exceed 1000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Level)
                .MaximumLength(50).WithMessage("Level can't exceed 50 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Level));

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Price must be greater than or equal to 0.")
                .When(x => x.Price.HasValue);

            RuleFor(x => x.Status)
                .MaximumLength(50).WithMessage("Status can't exceed 50 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Status));

            RuleFor(x => x.Duration)
                .GreaterThan(TimeSpan.Zero)
                .WithMessage("Duration must be greater than zero.")
                .When(x => x.Duration.HasValue);

            RuleFor(x => x.CatelogeId)
                .NotEmpty()
                .WithMessage("Category Id is required.");
        }
    }
}
