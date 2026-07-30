using GroupsCourseService.Dtos;
using GroupsCourseService.Model;

namespace GroupsCourseService.Service
{
    public interface IEnrollmentService
    {
        Task<List<EnrollmentResponseDto>> GetAllAsync();
        Task<EnrollmentResponseDto?> GetByIdAsync(Guid id);
        Task<GeneralResponse<EnrollmentResponseDto>> CreateAsync(CreateEnrollmentDto dto);
        Task<EnrollmentResponseDto?> UpdateGroupAsync(Guid id, UpdateEnrollmentDto dto);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}
