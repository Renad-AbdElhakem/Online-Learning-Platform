using Consul;

namespace CourseService.ExternalService
{
    public class InstructorServiceClient
    {
        private readonly IConsulClient _consulClient;
        private readonly HttpClient _httpClient;

        public InstructorServiceClient(IConsulClient consulClient,HttpClient httpClient)
        {
            _consulClient = consulClient;
            _httpClient = httpClient;
        }




        public async Task<string> GetInstructorByIdAsync(Guid instructorId) 
        {
    

            var service = await _consulClient.Health.Service("InstructorService");

            var instance = service.Response.FirstOrDefault();
            if (instance == null)
                throw new Exception($"Instructor with id {instructorId} not found");



            var url = $"http://{instance.Service.Address}/{instance.Service.Port}/api/instructorcourses/{instructorId}";

            var instructorClient = await _httpClient.GetAsync(url);

            return await instructorClient.Content.ReadAsStringAsync();

        }
    }
}
