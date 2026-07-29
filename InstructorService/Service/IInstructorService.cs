using InstructorService.Dtos;

namespace InstructorService.Service
{
    public interface IInstructorService
    {
        Task<IEnumerable<InstructorDto>> GetAllAsync();

        Task<InstructorDto?> GetByIdAsync(Guid id);

        Task<InstructorDto> CreateAsync(CreateInstructorDto dto);

        Task<bool> UpdateAsync(Guid id, UpdateInstructorDto dto);

        Task<bool> UpdateInstructorStatusAsync(Guid id, DeleteInstructorDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
