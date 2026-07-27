using Consul;
using System.Linq.Expressions;

namespace CourseService.ExternalService
{
    public class CategoryServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConsulClient _consulClient;

        public CategoryServiceClient(HttpClient httpClient, IConsulClient consulClient)
        {
            _httpClient = httpClient;
            _consulClient = consulClient;
        }



        public async Task<string> GetCatalogId(Guid CategoryId)
        {

            //************************

            var services = await _consulClient.Health.Service("categoriesService", tag: null, passingOnly: true);

            var instance = services.Response.FirstOrDefault();
            if (instance == null)
                throw new Exception("No category service instance found ");


            var url = $"http://{instance.Service.Address}:{instance.Service.Port}/api/categories/{CategoryId}";


            var response = await _httpClient.GetAsync(url);

            return  await response.Content.ReadAsStringAsync();


        }






    }
}
