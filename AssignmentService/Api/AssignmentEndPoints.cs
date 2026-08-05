using AssignmentService.Dtos;
using AssignmentService.Service;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentService.Api
{
    public static class AssignmentEndPoints
    {
        public static void MapAssignmentEndPoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/assignment");

            group.MapPost("", async ([FromForm] UploadNewAssignment newAssignment, IAssignmentService assignmentService) =>
            {
                var result = await assignmentService.UploadAssignmentAsync(newAssignment);

                return result.IsSuccseded ? Results.Ok(result) : Results.BadRequest(result.Message);

            }).DisableAntiforgery();


            group.MapPatch("{assignmentId}", async (Guid assignmentId, [FromForm] UpdateAssignmentDto updateAssignment, IAssignmentService assignmentService) =>
                {
                    var result = await assignmentService.UpdateAssignmentAsync(assignmentId, updateAssignment);

                    return result.IsSuccseded ? Results.Ok(result) : Results.NotFound(result.Message);

                }).DisableAntiforgery();


            group.MapDelete("{assignmentId}", async (Guid assignmentId, IAssignmentService assignmentService) =>
                {
                    var result = await assignmentService.DeleteAssignmentAsync(assignmentId);

                    return result ? Results.Ok() : Results.NoContent();
                });

            group.MapPatch("{assignmentId}/hide", async (Guid assignmentId, IAssignmentService assignmentService) =>
                {
                    var result = await assignmentService.HideAssignmentAsync(assignmentId);

                    return result ? Results.Ok("Assignment hidden successfully.") : Results.NotFound($"Assignment with id {assignmentId} not found.");
                });


            group.MapGet("download/{assignmentId}", async (Guid assignmentId, IAssignmentService assignmentService) =>
                {
                    var result = await assignmentService.DownloadAssignmentAsync(assignmentId);

                    if (!result.IsSuccseded)
                        return Results.NotFound(result.Message);

                    return Results.File(result.Data, "application/octet-stream", $"assignment_{assignmentId}");
                });


            group.MapGet("stream/{assignmentId}", async (Guid assignmentId, IAssignmentService assignmentService) =>
                {
                    var result = await assignmentService.StreamAssignmentAsync(assignmentId);

                    if (!result.IsSuccseded)
                        return Results.NotFound(result.Message);

                    return Results.Stream(result.Data, enableRangeProcessing: true);
                });
        }
    }
}
