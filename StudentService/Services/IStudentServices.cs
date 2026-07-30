using StudentService.Dtos;

namespace StudentService.Services
{
    public interface IStudentServices
    {
        Task<IEnumerable<StudentResponseDto>> GetAllAsync();
        Task<StudentResponseDto?> GetByIdAsync(Guid id);
        Task<StudentResponseDto> CreateAsync(CreateStudentDto dto);
        Task<StudentResponseDto?> UpdateStatusAsync(Guid id, UpdateStudentStatusDto dto);
        Task<GeneralResponse<StudentResponseDto>> ActivateAsync(Guid id);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}
