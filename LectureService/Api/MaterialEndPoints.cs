using Content_Service.Dtos;
using Content_Service.Service;
using Microsoft.AspNetCore.Mvc;

namespace Content_Service.Api
{
    public static class MaterialEndPoints
    {
        public static void MapMaterialEndPoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/material");

            group.MapPost("", async ([FromForm]AddNewLectureMaterialDto addNew, IMaterialService materialService) =>
            {

                var newMaterial = await materialService.AddNewMaterialAsync(addNew);
                return newMaterial.IsSuccseded ? Results.Ok(newMaterial.Data) : Results.BadRequest(newMaterial.Message);

            }).DisableAntiforgery();

            group.MapGet("/{lectureId}/Materials",async (Guid lectureId, IMaterialService materialService) =>
            {
                var materials = await materialService.GetAllLectureMaterials(lectureId);
                return Results.Ok(materials);
            });

            group.MapGet("Stream/{materialId}",async (Guid materialId, IMaterialService materialService) =>
            {
                var material = await materialService.StreamLectureMaterialByIdAsync(materialId);
                return Results.Stream(material.Data, "application/pdf", enableRangeProcessing: true);
            });

            group.MapGet("Download/{materialId}",async (Guid materialId, IMaterialService materialService) =>
            {
                var material = await materialService.DownloadLectureMaterialByIdAsync(materialId);
                return Results.File(material.Data, "application/pdf");
            });

            group.MapPatch("{materialId}", async (Guid materialId,[FromForm] UpdateLectureMaterial updateMaterial,IMaterialService materialService ) =>
            {
                var result = await materialService.UpdateLectureMaterials(materialId,updateMaterial);

                return result.IsSuccseded ? Results.Ok(result) : Results.NotFound(result.Message);
            }).DisableAntiforgery();

            group.MapDelete("{materialId}", async (Guid materialId, IMaterialService materialService) =>
            {
                var result = await materialService.DeleteLectureMaterialByIdAsync(materialId);
                return result ? Results.Ok() : Results.NoContent();
            });

            group.MapPatch("{materialId}/Hidden", async (Guid materialId, IMaterialService materialService) =>
            {
                var result = await materialService.HiddenLectureMaterialByIdAsync(materialId);
                return result ? Results.Ok() : Results.NotFound();
            });

        }
    }
}
