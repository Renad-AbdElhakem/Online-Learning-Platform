using CourseService.Dtos;
using CourseService.Model;

namespace CourseService.Service
{
    public interface IInstructorCoursesServices
    {
        Task<GeneralResponse<InstructorCourses>> CreateNewInstructorCourseAsync(CreateNewInstructorCourse createNew);
        Task<List<InstructorCourses>> GetAllInstructorCoursesAsync();

        Task<InstructorCourses?> GetInstructorCourseByIdAsync(int id);

        Task<GeneralResponse<InstructorCourses>> UpdateInstructorCourseInfoAsync(int id, UpdateInstructorCourse update);

        Task<GeneralResponse<InstructorCourses>> UpdateInstructorCourseStatusAsync(int id, UpdateInstructorCourseStatus update);

    }
}
