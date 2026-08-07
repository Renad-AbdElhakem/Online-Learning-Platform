using AssignmentService.Shared;
using Consul;
using System.Linq.Expressions;

namespace AssignmentService.ExternalService
{
    public class StudentClient
    {
        private readonly IConsulClient _consulClient;
        private readonly HttpClient _httpClient;

        public StudentClient(IConsulClient consulClient, HttpClient httpClient)
        {
            _consulClient = consulClient;
            _httpClient = httpClient;
        }


        public async Task<GeneralResponse<string>> GetByIdAsync(Guid studentId)
        {

            var service = await _consulClient.Health.Service("StudentService", passingOnly: true, tag: null);

            var instance = service.Response.FirstOrDefault();
            if (instance is null)
                return GeneralResponse<string>.Failed("No StudentService healthy found");

            var url = $"http://{instance.Service.Address}:{instance.Service.Port}/api/Student/{studentId}";

            var response = await _httpClient.GetAsync(url);

            try
            {
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    return GeneralResponse<string>.Success(data, null, response.StatusCode);

                }

                return GeneralResponse<string>.Failed(null, response.StatusCode);

            }
            catch (HttpRequestException ex)
            {
                return GeneralResponse<string>.Failed($"Network error : {ex.Message}");
            }

            catch (TaskCanceledException ex)
            {

                return GeneralResponse<string>.Failed($" request was cancelled or time out {ex.Message}");
            }

        }

    }








    }

