using CourseService.Dtos;
using CourseService.Model;

namespace CourseService.Service
{
    public interface ICourseServices
    {
        Task<GeneralResponse<ResponseCourse>> CreateNewCourseAsync(RequestNewCourse createNewCourse);
        Task<List<ResponseCourse>> GetAllCoursesAsync();

        Task<ResponseCourse?> GetCourseByIdAsync(Guid id);

        Task<bool> SoftDeleteCourseAsync(Guid id);

        Task<GeneralResponse<ResponseCourse>> UpdateCourseAsync(Guid id, RequestUpdateCourse request);
    }
}
