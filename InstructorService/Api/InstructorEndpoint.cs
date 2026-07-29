using InstructorService.Dtos;
using InstructorService.Service;
using System.Runtime.CompilerServices;

namespace InstructorService.Api
{
    public static class InstructorEndpoint
    {

        public static void MapInstructorEndpoint(this WebApplication app)
        {

            var group = app.MapGroup("/api/instructor/");

      
            group.MapGet("", async (IInstructorService service) =>
            {
                var instructors = await service.GetAllAsync();
                return Results.Ok(instructors);
            });

       
            group.MapGet("{id}", async (Guid id, IInstructorService service) =>
            {
                var instructor = await service.GetByIdAsync(id);

                return instructor == null  ? Results.NotFound() : Results.Ok(instructor);
            });

       
            group.MapPost("", async (CreateInstructorDto dto, IInstructorService service) =>
            {
                var instructor = await service.CreateAsync(dto);

                return Results.Created($"/api/instructor/{instructor.Id}", instructor);
            });
       
            group.MapPatch("", async (Guid id, UpdateInstructorDto dto, IInstructorService service) =>
            {
                var updated = await service.UpdateAsync(id, dto);

                return updated  ? Results.NoContent() : Results.NotFound();
            });


            group.MapPatch("/{id}", async (Guid id, DeleteInstructorDto dto, IInstructorService service) =>
            {
                var deleted = await service.UpdateInstructorStatusAsync(id, dto);

                return deleted ? Results.NoContent() : Results.NotFound();
            });
           
            group.MapDelete("{id}", async (Guid id, IInstructorService service) =>
            {
                var deleted = await service.DeleteAsync(id);

                return deleted ? Results.NoContent() : Results.NotFound();
            });




         
        }
    }
}
