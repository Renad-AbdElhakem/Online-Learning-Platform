using StudentService.Dtos;
using StudentService.Services;

namespace StudentService.Api
{
    public  static class StudentEndPoints
    {
        public static void MapStudentEndPoints(this WebApplication app) 
        {

            var group = app.MapGroup("/api/Student/");


            group.MapPost("", async (CreateStudentDto dto, IStudentServices service) =>
            {
                var result = await service.CreateAsync(dto);
                return Results.Created($"/api/Student/{result.Id}", result);
            });


            group.MapGet("", async (IStudentServices service) =>
            {
                var result = await service.GetAllAsync();
                return Results.Ok(result);
            });

            group.MapGet("{id}", async (Guid id, IStudentServices service) =>
            {
                var result = await service.GetByIdAsync(id);
                return result is null ? Results.NotFound() : Results.Ok(result);
            });

           
            group.MapPatch("{id}/status", async (Guid id, UpdateStudentStatusDto dto, IStudentServices service) =>
            {
                var result = await service.UpdateStatusAsync(id, dto);
                return result is null ? Results.NotFound() : Results.Ok(result);
            });


            group.MapPatch("{id}/activate", async (Guid id, IStudentServices service) =>
            {
                var result = await service.ActivateAsync(id);
                return !result.IsSuccseded ? Results.NotFound(result.ErrorMessage) : Results.Ok(result);
            });

                group.MapDelete("{id}", async (Guid id, IStudentServices service) =>
            {
                var success = await service.SoftDeleteAsync(id);
                return success ? Results.NoContent() : Results.NotFound();
            });
        }
    }
}
