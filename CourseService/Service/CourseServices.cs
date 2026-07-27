using CourseService.Data;
using CourseService.Dtos;
using CourseService.ExternalService;
using CourseService.Model;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Service
{
    public class CourseServices : ICourseServices
    {
        private readonly CourseDBContext _dBContext;
        private readonly CategoryServiceClient _categoryServiceClient;

        public CourseServices(CourseDBContext dBContext, CategoryServiceClient categoryServiceClient)
        {
            _dBContext = dBContext;
            _categoryServiceClient = categoryServiceClient;
        }




        public async Task<string?> CreateNewCourseAsync(RequestNewCourse createNewCourse)
        {

            var category = await _categoryServiceClient.GetCatalogId(createNewCourse.CatelogeId);
            if (category == null)
                return null;

            var newCourse = new Course
            {
                Name = createNewCourse.Name,
                Status = createNewCourse.Status,
                Description = createNewCourse.Description,
                Duration = createNewCourse.Duration,
                CatelogeId = createNewCourse.CatelogeId,
                CreatedAt = createNewCourse.CreatedAt,
                IsActive = createNewCourse.IsActive,
                Level = createNewCourse.Level,
                Price = createNewCourse.Price,

            };
            await _dBContext.Courses.AddAsync(newCourse);

            await _dBContext.SaveChangesAsync();

            return "New course added suscessfuly";
        }

        public async Task<List<ResponseCourse>> GetAllCoursesAsync()
        {
                return await _dBContext.Courses
           .Where(c => c.IsActive == true)
           .Select(c => new ResponseCourse
           {
               Id = c.Id,
               Name = c.Name,
               Description = c.Description,
               Level = c.Level,
               Price = c.Price,
               IsActive = c.IsActive,
               Status = c.Status,
               CreatedAt = c.CreatedAt,
               Duration = c.Duration,
               CatelogeId = c.CatelogeId
           })
           .ToListAsync();
        }

        public async Task<ResponseCourse?> GetCourseByIdAsync(Guid id)
        {
                return await _dBContext.Courses
          .Where(c => c.Id == id && c.IsActive == true)
          .Select(c => new ResponseCourse
          {
              Id = c.Id,
              Name = c.Name,
              Description = c.Description,
              Level = c.Level,
              Price = c.Price,
              IsActive = c.IsActive,
              Status = c.Status,
              CreatedAt = c.CreatedAt,
              Duration = c.Duration,
              CatelogeId = c.CatelogeId
          })
          .FirstOrDefaultAsync();
            }

        public  async Task<bool> SoftDeleteCourseAsync(Guid id)
        {
            var course = await _dBContext.Courses.FindAsync(id);

            if (course == null || course.IsActive == false)
                return false;

            course.IsActive = false;

            await _dBContext.SaveChangesAsync();

            return true;
        }

        public async Task<string?> UpdateCourseAsync(Guid id, RequestUpdateCourse request)
        {
            var course = await _dBContext.Courses.FindAsync(id);

            if (course == null || course.IsActive == false)
                return null;

            if (request.Name != null)
                course.Name = request.Name;

            if (request.Description != null)
                course.Description = request.Description;

            if (request.Level != null)
                course.Level = request.Level;

            if (request.Price.HasValue)
                course.Price = request.Price;

            if (request.Status != null)
                course.Status = request.Status;

            if (request.Duration.HasValue)
                course.Duration = request.Duration;

            if (request.IsActive.HasValue)
                course.IsActive = request.IsActive;

            await _dBContext.SaveChangesAsync();

            return $"Course with id {id} Updated";
        }
    }
}
