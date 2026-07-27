using CourseService.Dtos;

namespace CourseService.Service
{
    public interface ICourseServices
    {
        Task<string?> CreateNewCourseAsync(RequestNewCourse createNewCourse);
        Task<List<ResponseCourse>> GetAllCoursesAsync();

        Task<ResponseCourse?> GetCourseByIdAsync(Guid id);

        Task<bool> SoftDeleteCourseAsync(Guid id);

        Task<string?> UpdateCourseAsync(Guid id, RequestUpdateCourse request);
    }
}
