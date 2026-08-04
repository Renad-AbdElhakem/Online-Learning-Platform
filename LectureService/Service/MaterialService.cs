using Content_Service.Dtos;
using Content_Service.Model;
using LectureService.Data;
using LectureService.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Content_Service.Service
{
    public class MaterialService : IMaterialService
    {
        private readonly ContentDbContext _dbContext;
        private readonly ILectureService _lectureService;

        public MaterialService(ContentDbContext dbContext, ILectureService lectureService)
        {
            _dbContext = dbContext;
            _lectureService = lectureService;
        }

        public async Task<GeneralResponse<Material>> AddNewMaterialAsync(AddNewLectureMaterialDto addNewMaterial)
        {

            if (!await _lectureService.LectureExistsAsync(addNewMaterial.LectureId))
                return GeneralResponse<Material>.Failed($"Lecture with id {addNewMaterial.LectureId} not found");


            if (addNewMaterial.LectureMaterial.Length == 0)
                return GeneralResponse<Material>.Failed($"No file received.");

            if (!AllowedExtension(addNewMaterial.LectureMaterial))
                return GeneralResponse<Material>.Failed("File type not allowed.");


            var materialFile = await SaveLectureMaterialAsync(addNewMaterial.LectureMaterial, addNewMaterial.MaterialName);

            var newMaterial = new Material
            {
                LectureId = addNewMaterial.LectureId,
                LectureMaterialPath = materialFile,
                MaterialName = addNewMaterial.MaterialName,
                Description = addNewMaterial.Description,
                UploadedAt = DateTime.Now,
                IsVisible = addNewMaterial.IsVisible,

            };
            await _dbContext.Materials.AddAsync(newMaterial);
            await _dbContext.SaveChangesAsync();

            return GeneralResponse<Material>.Success(newMaterial, "done");
        }



        public async Task<GeneralResponse<List<MaterialResponseDto>>> GetAllLectureMaterials(Guid lectureId)
        {
            if (!await _dbContext.Materials.AnyAsync(m => m.LectureId == lectureId))
                return GeneralResponse<List<MaterialResponseDto>>.Failed($"No lecture found with id {lectureId}");

            var lectureMaterials = await _dbContext.Materials.Where(m => m.LectureId == lectureId).ToListAsync();

            if (!lectureMaterials.Any())
                return GeneralResponse<List<MaterialResponseDto>>.Failed($"No material uploaded for lecture {lectureId}");

            var materials = lectureMaterials.Select(m =>
            new MaterialResponseDto
            {
                Id = m.Id,
                MaterialName = m.MaterialName,
                Description = m.Description,
                UploadedAt = m.UploadedAt,
                ModifiedAt = m.ModifiedAt,
                LectureMaterialPath = m.LectureMaterialPath,
            }).ToList();


            return GeneralResponse<List<MaterialResponseDto>>.Success(materials);

        }
        public async Task<GeneralResponse<MaterialResponseDto>> UpdateLectureMaterials(Guid materialId, UpdateLectureMaterial updateMaterial)
        {

            var lectureMaterial = await _dbContext.Materials.FindAsync(materialId);

            if (lectureMaterial is null)
                return GeneralResponse<MaterialResponseDto>.Failed($"No material uploaded found with id {materialId}");

            if (!string.IsNullOrEmpty(updateMaterial.MaterialName))
                lectureMaterial.MaterialName = updateMaterial.MaterialName;

            if (!string.IsNullOrEmpty(updateMaterial.Description))
                lectureMaterial.Description = updateMaterial.Description;

            if (updateMaterial.LectureId.HasValue)
            {
                if (await _lectureService.LectureExistsAsync(updateMaterial.LectureId.Value))
                { return GeneralResponse<MaterialResponseDto>.Failed($"No lecture found with id {updateMaterial.LectureId.Value}"); }
                lectureMaterial.LectureId = updateMaterial.LectureId.Value;
            }


            if (updateMaterial.LectureMaterialPath != null)
            {
                if (!AllowedExtension(updateMaterial.LectureMaterialPath))
                    return GeneralResponse<MaterialResponseDto>.Failed("File type not allowed.");

                var materialLocation = await SaveLectureMaterialAsync(updateMaterial.LectureMaterialPath, lectureMaterial.MaterialName);
                lectureMaterial.LectureMaterialPath = materialLocation;
            }
            await _dbContext.SaveChangesAsync();

            var material = new MaterialResponseDto
            {
                Id = lectureMaterial.Id,
                MaterialName = lectureMaterial.MaterialName,
                LectureMaterialPath = lectureMaterial.LectureMaterialPath,
                Description = lectureMaterial.Description,
                ModifiedAt = DateTime.Now,
            };

            return GeneralResponse<MaterialResponseDto>.Success(material);

        }

        public async Task<GeneralResponse<Stream>> StreamLectureMaterialByIdAsync(Guid materialId)
        {
            var material = await _dbContext.Materials.FindAsync(materialId);
            if (material is null)
                return GeneralResponse<Stream>.Failed($"No material with id {materialId} found");

            if (!File.Exists(material.LectureMaterialPath))
                return GeneralResponse<Stream>.Failed("File not found.");

            var stream = File.OpenRead(material.LectureMaterialPath);
            return GeneralResponse<Stream>.Success(stream, "Stream ready.");

        }

        public async Task<GeneralResponse<byte[]>> DownloadLectureMaterialByIdAsync(Guid materialId)
        {
            var material = await _dbContext.Materials.FindAsync(materialId);
            if (material is null)
                return GeneralResponse<byte[]>.Failed($"No material with id {materialId} found");

            if (!File.Exists(material.LectureMaterialPath))
                return GeneralResponse<byte[]>.Failed("File not found.");


            var materialBytes = await File.ReadAllBytesAsync(material.LectureMaterialPath);

            return GeneralResponse<byte[]>.Success(materialBytes, "File ready for download.");

        }


        public async Task<bool> DeleteLectureMaterialByIdAsync(Guid materialId)
        {
            var material = await _dbContext.Materials.FindAsync(materialId);
            if (material is null)
                return false;

            _dbContext.Materials.Remove(material);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> HiddenLectureMaterialByIdAsync(Guid materialId)
        {
            var material = await _dbContext.Materials.FindAsync(materialId);
            if (material is null)
                return false;

            material.IsVisible = false;
            await _dbContext.SaveChangesAsync();
            return true;
        }



        private bool AllowedExtension(IFormFile file)
        {

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return false;

            return true;
        }

        private async Task<string> SaveLectureMaterialAsync(IFormFile lectureMaterial, string materialName)
        {
            var fileName = $"{Guid.NewGuid().ToString().Substring(0, 5)}_{materialName}{Path.GetExtension(lectureMaterial.FileName)}";

            var materialfolderPath = Path.Combine(Directory.GetCurrentDirectory(),
                                             "Uploads", "Materials");

            Directory.CreateDirectory(materialfolderPath);

            var material_FilePath = Path.Combine(materialfolderPath, fileName);

            using var stream = new FileStream(material_FilePath, FileMode.Create);

            await lectureMaterial.CopyToAsync(stream);

            return material_FilePath;


        }

    }
}
