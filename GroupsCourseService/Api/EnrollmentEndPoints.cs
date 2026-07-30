using GroupsCourseService.Dtos;
using GroupsCourseService.Service;
using System.Diagnostics;

namespace GroupsCourseService.Api
{
    public static class EnrollmentEndPoints
    {
        public static void MapEnrollmentEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/enrollment/");

            group.MapPost("", async (CreateEnrollmentDto dto, IEnrollmentService service) =>
            {
                var result = await service.CreateAsync(dto);

                return result.IsSuccseded ? Results.Created($"/api/enrollment/{result.Data.Id}", result) : Results.BadRequest(result.ErrorMessage);
            });


            group.MapGet("", async (IEnrollmentService service) =>
            {
                var result = await service.GetAllAsync();
                return Results.Ok(result);
            });

            group.MapGet("{id}", async (Guid id, IEnrollmentService service) =>
            {
                var result = await service.GetByIdAsync(id);
                return result is null ? Results.NotFound() : Results.Ok(result);
            });

            group.MapPatch("{id}", async (Guid id, UpdateEnrollmentDto dto, IEnrollmentService service) =>
            {
                var result = await service.UpdateGroupAsync(id, dto);
                return result is null ? Results.NotFound() : Results.Ok(result);
            });

            group.MapDelete("{id}", async (Guid id, IEnrollmentService service) =>
            {
                var success = await service.SoftDeleteAsync(id);
                return success ? Results.NoContent() : Results.NotFound();
            });
        }
    }
}
