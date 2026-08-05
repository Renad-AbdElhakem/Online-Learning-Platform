using AssignmentService.Dtos;
using AssignmentService.Service;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentService.Api
{
    public static class StudentAssignmentEndPoints
    {
        public static void MapStudentAssignmentEndPoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/studentassignment");



            group.MapPost("", async ([FromForm] SubmittedAssignmentDto dto, IStudentAssignmentService assignmentService) =>
            {
                var result = await assignmentService.SubmittedNewAssignment(dto);

                return result.IsSuccseded ? Results.Ok(result) : Results.BadRequest(result.Message);

            }).DisableAntiforgery();


            group.MapPatch("grade/{assignmentStudentId}", async (Guid assignmentStudentId, GradeAssignmentDto gradeAssignment,
                                                                         IStudentAssignmentService assignmentService) =>
            {
                var result = await assignmentService.GradeAssignmentAsync(assignmentStudentId, gradeAssignment);

                return result.IsSuccseded ? Results.Ok(result) : Results.NotFound(result.Message);
            });



            group.MapGet("{assignmentStudentId}", async (Guid assignmentStudentId, IStudentAssignmentService assignmentService) =>
            {
                var result = await assignmentService.GetStudentAssignmentByIdAsync(assignmentStudentId);

                return result.IsSuccseded ? Results.Ok(result) : Results.NotFound(result.Message);
            });


            group.MapGet("student-assignment/download/{assignmentStudentId}", async (Guid assignmentStudentId, IStudentAssignmentService assignmentService) =>
            {
                var result = await assignmentService.DownloadStudentAssignmentAsync(assignmentStudentId);

                if (!result.IsSuccseded)
                    return Results.NotFound(result.Message);

                return Results.File(result.Data, "application/octet-stream", $"student-assignment-{assignmentStudentId}");
            });

            group.MapGet("student-assignment/stream/{assignmentStudentId}", async (Guid assignmentStudentId, IStudentAssignmentService assignmentService) =>
            {
                var result = await assignmentService.StreamStudentAssignmentAsync(assignmentStudentId);

                if (!result.IsSuccseded)
                    return Results.NotFound(result.Message);

                return Results.Stream(result.Data, "application/octet-stream", enableRangeProcessing: true);
            });

        }


    }
}
