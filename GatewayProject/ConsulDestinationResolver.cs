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

            foreach (var (key, config) in destinations)
            {
                
                var services = await _consulClient.Health.Service(
                    config.Host!,
                    tag: null,
                    passingOnly: true,
                    cancellationToken
                );

                foreach (var service in services.Response)
                {
                    var address = service.Service.Address;
                    var port = service.Service.Port;
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
/*
 * Key

catalog

↓

Value

Host = CatalogService

 "Clusters": {
  "catalog-cluster": {
    "Destinations": {
      "catalog1": {
        "Host": "CatalogService"
      },
      "catalog2": {
        "Host": "CatalogService"
      }
    }
  }
}

 */