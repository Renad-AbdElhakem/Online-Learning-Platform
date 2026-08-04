using Content_Service.Dtos;
using Content_Service.Model;

namespace Content_Service.Service
{
    public interface IMaterialService
    {
        Task<GeneralResponse<Material>> AddNewMaterialAsync(AddNewLectureMaterialDto addNewMaterial);
        Task<GeneralResponse<List<MaterialResponseDto>>> GetAllLectureMaterials(Guid lectureId);
        Task<GeneralResponse<Stream>> StreamLectureMaterialByIdAsync(Guid materialId);
        Task<GeneralResponse<byte[]>> DownloadLectureMaterialByIdAsync(Guid materialId);
        Task<bool> DeleteLectureMaterialByIdAsync(Guid materialId);
        Task<bool> HiddenLectureMaterialByIdAsync(Guid materialId);
        Task<GeneralResponse<MaterialResponseDto>> UpdateLectureMaterials(Guid materialId, UpdateLectureMaterial updateMaterial);
    }
}
