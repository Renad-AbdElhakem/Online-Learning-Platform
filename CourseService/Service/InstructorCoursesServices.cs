using CourseService.Data;
using CourseService.Dtos;
using CourseService.ExternalService;
using CourseService.Model;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace CourseService.Service
{
    public class InstructorCoursesServices : IInstructorCoursesServices
    {
        private readonly ICourseServices _courseServices;
        private readonly CourseDBContext _dBContext;
        private readonly InstructorServiceClient _instructorClient;

        public InstructorCoursesServices(ICourseServices courseServices, CourseDBContext dBContext, InstructorServiceClient instructorClient)
        {
            _courseServices = courseServices;
            _dBContext = dBContext;
            _instructorClient = instructorClient;
        }

        public async Task<GeneralResponse<InstructorCourses>> CreateNewInstructorCourseAsync(CreateNewInstructorCourse createNew)
        {

            var instructor = await _instructorClient.GetInstructorByIdAsync(createNew.InstructorId);

            if (string.IsNullOrEmpty(instructor))
                return GeneralResponse<InstructorCourses>.Failed($"Instructor with id {createNew.InstructorId} not found");

            var course = await _courseServices.GetCourseByIdAsync(createNew.CourseId);

            if (course == null)
                return GeneralResponse<InstructorCourses>.Failed($"Course With Id {createNew.CourseId} not found");


            var newInstructorCourse = new InstructorCourses
            {
             
                IsActive = createNew.IsActive,
                status = createNew.status,
                InstructorId = createNew.InstructorId,
                CourseId = createNew.CourseId,

            };

            await _dBContext.instructorCourses.AddAsync(newInstructorCourse);
            await _dBContext.SaveChangesAsync();

            return GeneralResponse<InstructorCourses>.Successful(newInstructorCourse, "New instructor course added successfully");
        }


        public async Task<List<InstructorCourses>> GetAllInstructorCoursesAsync()
        {
            return await _dBContext.instructorCourses.ToListAsync();
        }
        public async Task<InstructorCourses?> GetInstructorCourseByIdAsync(int id)
        {
            return await _dBContext.instructorCourses
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<GeneralResponse<InstructorCourses>> UpdateInstructorCourseInfoAsync(int id, UpdateInstructorCourse update)
        {
            var instructorCourse = await _dBContext.instructorCourses
                .FirstOrDefaultAsync(x => x.Id == id);

            if (instructorCourse == null)
                return GeneralResponse<InstructorCourses>.Failed($"Instructor Course with id {id} not found");

            var instructor = await _instructorClient.GetInstructorByIdAsync(update.InstructorId);

            if (string.IsNullOrEmpty(instructor))
                return GeneralResponse<InstructorCourses>.Failed($"Instructor With Id {update.InstructorId} not found");

            var course = await _courseServices.GetCourseByIdAsync(update.CourseId);

            if (course == null)
                return GeneralResponse<InstructorCourses>.Failed($"Course With Id {update.CourseId} not found");

            instructorCourse.InstructorId = update.InstructorId;
            instructorCourse.CourseId = update.CourseId;

            await _dBContext.SaveChangesAsync();


            return GeneralResponse<InstructorCourses>.Successful(instructorCourse,"Updated");
        }


        public async Task<GeneralResponse<InstructorCourses>> UpdateInstructorCourseStatusAsync(int id, UpdateInstructorCourseStatus update)
        {
            var instructorCourse = await _dBContext.instructorCourses
                .FirstOrDefaultAsync(x => x.Id == id);

            if (instructorCourse == null)
                return GeneralResponse<InstructorCourses>.Failed("Instructor Course not found");

            instructorCourse.IsActive = update.IsActive;
            instructorCourse.status = update.Status;

            await _dBContext.SaveChangesAsync();

            return GeneralResponse<InstructorCourses>.Successful(instructorCourse,"Updates");
        }
    }
}
