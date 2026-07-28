using CourseService.Dtos;
using FluentValidation;

namespace CourseService.Validator
{
    public class CreateNewInstructorCourseValidation : AbstractValidator<CreateNewInstructorCourse>
    {
        public CreateNewInstructorCourseValidation()
        {
            RuleFor(x => x.InstructorId)
           .NotEmpty()
           .WithMessage("Instructor Id is required.");

            RuleFor(x => x.CourseId)
                .NotEmpty()
                .WithMessage("Course Id is required.");

            RuleFor(x => x.status)
                .MaximumLength(50)
                .WithMessage("Status can't exceed 50 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.status));
        }
    }
}
