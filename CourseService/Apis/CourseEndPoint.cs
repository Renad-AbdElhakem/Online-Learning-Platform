using CourseService.Data;
using CourseService.Dtos;
using CourseService.ExternalService;
using CourseService.Model;
using CourseService.Service;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace CourseService.CourseApi
{
    public static class CourseEndPoint
    {


        public static void MapCourseEndPoint(this WebApplication app)
        {
            var group = app.MapGroup("/api/Courses");



            group.MapPost("", async (RequestNewCourse createNewCourse,IValidator<RequestNewCourse> validator, ICourseServices courseServices) =>
            {

                var validatorReesult = await validator.ValidateAsync(createNewCourse);
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
                var newCourse = await courseServices.CreateNewCourseAsync(createNewCourse);
                if (newCourse == null)
                    return Results.BadRequest(new { message = $"Category with id {createNewCourse.CatelogeId} not found" });

                else
                    return Results.Ok("New course addrd sucessfully");
            });


            group.MapGet("", async (ICourseServices courseServices) =>
            {
                var courses = await courseServices.GetAllCoursesAsync();

                return Results.Ok(courses);
            });



            group.MapGet("/{id:guid}", async (Guid id, ICourseServices courseServices) =>
            {
                var course = await courseServices.GetCourseByIdAsync(id);

                if (course == null)
                    return Results.NotFound();

                return Results.Ok(course);
            });



            group.MapDelete("/{id:guid}", async (Guid id, ICourseServices courseServices) =>
            {
                var result = await courseServices.SoftDeleteCourseAsync(id);

                if (!result)
                    return Results.NotFound();

                return Results.Ok("Course deleted successfully");
            });



            group.MapPatch("/{id:guid}", async (Guid id, RequestUpdateCourse request, ICourseServices courseServices) =>
            {
                var result = await courseServices.UpdateCourseAsync(id, request);

                if (result == null)
                    return Results.NotFound();

                return Results.Ok("Course updated successfully");
            });



        }

    }
}
