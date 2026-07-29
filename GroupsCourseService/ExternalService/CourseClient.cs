using Consul;
using GroupsCourseService.Data;
using GroupsCourseService.Model;

namespace GroupsCourseService.ExternalService
{
    public class CourseClient
    {
        private readonly GroupsCourseDbContext _context;
        private readonly IConsulClient _consulClient;
        private readonly HttpClient _httpClient;

        public CourseClient(GroupsCourseDbContext context, IConsulClient consulClient, HttpClient httpClient)
        {
            _context = context;
            _consulClient = consulClient;
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse<string>> GetByIdAsync(Guid CourseId)
        {

            var service = await _consulClient.Health.Service("CoursesService");

            var instance = service.Response.FirstOrDefault();

            var url = $"http://{instance.Service.Address}:{instance.Service.Port}/api/Courses/{CourseId}";

            var courseClient = await _httpClient.GetAsync(url);

            try
            {
                if (courseClient.IsSuccessStatusCode)
                {
                    var data = await courseClient.Content.ReadAsStringAsync();
                    return GeneralResponse<string>.Succsess(data, null, courseClient.StatusCode);
                }

                return GeneralResponse<string>.failed(null, courseClient.StatusCode);
            }
           
            catch (HttpRequestException ex)
            {
                return GeneralResponse<string>.failed($"Network error : {ex.Message}");
            }

            catch (TaskCanceledException ex)
            {

                return GeneralResponse<string>.failed($" request was cancelled or time out {ex.Message}");
            }
        }







    }
}
