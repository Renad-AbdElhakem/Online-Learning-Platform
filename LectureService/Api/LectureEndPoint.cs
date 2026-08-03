using Content_Service.Dtos;
using Content_Service.Service;
using LectureService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Content_Service.Api
{
    public static class LectureEndPoint
    {

        public static void MapLectureEndPoint(this WebApplication app)
        {
            var group = app.MapGroup("/api/lecture");

            group.MapPost("", async ([FromForm] UploadNewLecture newLecture, ILectureService lectureService) =>
            {

                var result = await lectureService.UploadLectureVideo(newLecture);
                return result.IsSuccseded ? Results.Ok(result) : Results.BadRequest(result.Message);


            }).DisableAntiforgery(); ;


            group.MapPatch("{lectureId}", async (Guid lectureId, UpateLectureDto upateLecture, ILectureService lectureService) =>
            {
                var result = await lectureService.UpdateLectureAsync(lectureId, upateLecture);
              
                return result.IsSuccseded? Results.Ok(result): Results.NotFound(result.Message) ;
            });



            group.MapDelete("{lectureId}", async (Guid lectureId, ILectureService lectureService) =>
            {
                var result = await lectureService.DeleteLecture(lectureId);
                return result ? Results.Ok() : Results.NoContent();
            });

            group.MapGet("download/{lectureId}", async (Guid lectureId, ILectureService lectureService) =>
            {
                var result = await lectureService.DownloadLectureVideo(lectureId);
                if (!result.IsSuccseded) return Results.NotFound(result.Message);

                return Results.File(result.Data, "video/mp4", $"lecture_{lectureId}.mp4");
            });


            group.MapGet("stream/{lectureId}", async (Guid lectureId, ILectureService lectureService) =>
            {
                var result = await lectureService.StreamLectureVideo(lectureId);
                if (!result.IsSuccseded) return Results.NotFound(result.Message);

                return Results.Stream(result.Data, "video/mp4", enableRangeProcessing: true);
            });
        }
    }
}
