using CourseService.Dtos;
using CourseService.Service;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CourseService.Apis
{
    public static class InstructorCoursesEndPoint
    {


        public static void MapInstarctorCoursesEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/instructorCourses");

            group.MapPost("", async (CreateNewInstructorCourse createNew, IValidator<CreateNewInstructorCourse> validator
                                                                        , IInstructorCoursesServices instructorCoursesServices) =>
            {

                var validatorReesult = await validator.ValidateAsync(createNew);
                if (!validatorReesult.IsValid)
                {
                    return Results.BadRequest(new
                    {
                        errors = validatorReesult.Errors.Select(e => new
                        {
                            propertyfield = e.PropertyName,
                            message = e.ErrorMessage
                        })
                    });
                }
                var newCourse = await instructorCoursesServices.CreateNewInstructorCourseAsync(createNew);
                if (!newCourse.IsSucceeded)
                    return Results.BadRequest(newCourse.Message);

                else return Results.Ok(newCourse);

            });



            group.MapGet("", async (IInstructorCoursesServices service) =>
            {
                var data = await service.GetAllInstructorCoursesAsync();

                return Results.Ok(data);
            });

            group.MapGet("/{id}", async (int id, IInstructorCoursesServices service) =>
            {
                var data = await service.GetInstructorCourseByIdAsync(id);

                if (data == null)
                    return Results.NotFound();

                return Results.Ok(data);
            });

            group.MapPatch("/{id}/info", async (int id, UpdateInstructorCourse update, IInstructorCoursesServices service) =>
            {
                var updateResult = await service.UpdateInstructorCourseInfoAsync(id, update);
                if (!updateResult.IsSucceeded)
                    return Results.BadRequest(updateResult.Message);
                else
                    return Results.Ok(updateResult.Message);
            });


            group.MapPatch("/{id}/status", async (int id, UpdateInstructorCourseStatus update, IInstructorCoursesServices service) =>
            {
                var updateResult = await service.UpdateInstructorCourseStatusAsync(id, update);

                if (!updateResult.IsSucceeded)
                    return Results.BadRequest(updateResult.Message);
                else
                    return Results.Ok("Updated Successfully");
            });

        }


    }
}
