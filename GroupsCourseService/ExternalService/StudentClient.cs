using Consul;
using GroupsCourseService.Data;
using GroupsCourseService.Model;

namespace GroupsCourseService.ExternalService
{
    public class StudentClient
    {
        private readonly GroupsCourseDbContext _context;
        private readonly IConsulClient _consulClient;
        private readonly HttpClient _httpClient;

        public StudentClient(GroupsCourseDbContext context, IConsulClient consulClient, HttpClient httpClient)
        {
            _context = context;
            _consulClient = consulClient;
            _httpClient = httpClient;
        }



        public async Task<GeneralResponse<string>> GetByIdAsync(Guid studentId)
        {

            var service = await _consulClient.Health.Service("StudentService");

            var instance = service.Response.FirstOrDefault();

            var url = $"http://{instance.Service.Address}:{instance.Service.Port}/api/students/{studentId}";

            var instructorClient = await _httpClient.GetAsync(url);

            try
            {
                if (instructorClient.IsSuccessStatusCode)
                {
                    var data = await instructorClient.Content.ReadAsStringAsync();
                    return GeneralResponse<string>.Succsess(data, null, instructorClient.StatusCode);

                }

                return GeneralResponse<string>.failed(null, instructorClient.StatusCode);

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
