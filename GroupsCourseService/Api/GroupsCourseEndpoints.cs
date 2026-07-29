using GroupsCourseService.Dtos;
using GroupsCourseService.Service;

namespace GroupsCourseService.Api
{
    public static class GroupsCourseEndpoints
    {

        public static void RegisterRoutes(this WebApplication app)
        {

            var group = app.MapGroup("/api/Groups/");

            group.MapPost("", async (RequestCreateNewGroup newGroup, IGroupService groupService) =>
            {

                var result = await groupService.CreateNewGroup(newGroup);

                return result.IsSuccseded ? Results.Ok(result) : Results.BadRequest(result);
            });

            group.MapGet("", async (IGroupService groupService) =>
            {
                var result = await groupService.GetAllAsync();

                return Results.Ok(result) ;
            });

            group.MapGet("/{id}", async (Guid id, IGroupService groupService) =>
            {
                var result = await groupService.GetByIdAsync(id);

                return result.IsSuccseded
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            });

            group.MapPatch("/{id}/complete", async (Guid id, IGroupService groupService) =>
            {
                var result = await groupService.CompleteGroupAsync(id);

                return result.IsSuccseded
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            });

            group.MapPatch("/{id}/status", async (Guid id, UpdateGroupStatusDto dto,IGroupService groupService) =>
            {
                var result = await groupService.UpdateStatusGroupAsync(id,dto);

                return result.IsSuccseded
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            });

            group.MapPatch("/{id:guid}/assignment", async ( Guid id, UpdateGroupAssignmentDto dto, IGroupService groupService) =>
            {
                var result = await groupService.UpdateGroupAssignmentAsync(id, dto);

                return result.IsSuccseded
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            });
        }


    }
}
