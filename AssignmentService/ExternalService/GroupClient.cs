using AssignmentService.Shared;
using Consul;

namespace AssignmentService.ExternalService
{
    public class GroupClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConsulClient _consul;

        public GroupClient(HttpClient httpClient, IConsulClient consul)
        {
            _httpClient = httpClient;
            _consul = consul;
        }


        public async Task<GeneralResponse<string>> GetByIdAsync(Guid groupId)
        {

            var service = await _consul.Health.Service("GroupsService", passingOnly: true, tag: null);

            var instance = service.Response.FirstOrDefault();

            
            var url = $"http://{instance.Service.Address}:{instance.Service.Port}/api/Groups/{groupId}";

            var groupClient = await _httpClient.GetAsync(url);

            try
            {
                if (groupClient.IsSuccessStatusCode)
                {
                    var data = await groupClient.Content.ReadAsStringAsync();
                    return GeneralResponse<string>.Success(data, null, groupClient.StatusCode);

                }

                return GeneralResponse<string>.Failed(null, groupClient.StatusCode);

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
