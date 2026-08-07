using Consul;
using Microsoft.Extensions.FileProviders;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.ServiceDiscovery;

namespace GatewayProject
{
    public class ConsulDestinationResolver : IDestinationResolver
    {
        private readonly IConsulClient _consulClient;

        public ConsulDestinationResolver(IConsulClient consulClient)
        {
            _consulClient = consulClient;
        }

        public async ValueTask<ResolvedDestinationCollection> ResolveDestinationsAsync
    (IReadOnlyDictionary<string, Yarp.ReverseProxy.Configuration.DestinationConfig> destinations, CancellationToken cancellationToken)
        {
            var resolved = new Dictionary<string, Yarp.ReverseProxy.Configuration.DestinationConfig>();


            var allServices = await _consulClient.Agent.Services();

            foreach (var (key, config) in destinations)
            {

                var matchedServices = allServices.Response.Values
                    .Where(s => s.Service.Equals(config.Address,
                           StringComparison.OrdinalIgnoreCase));

                foreach (var service in matchedServices)
                {
                    var address = service.Address;
                    var port = service.Port;
                    Console.WriteLine($"Resolved: {config.Address} → {address}:{port}");
                    var destKey = $"{key}_{address}_{port}";
                    resolved[destKey] = config with
                    {
                        Address = $"http://{address}:{port}"
                    };
                }
            }

            return new ResolvedDestinationCollection(
                resolved,
                NullChangeToken.Singleton
            );
        }

    }
}
