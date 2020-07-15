using Microsoft.Extensions.DependencyInjection;

namespace Services
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddServicesConnector(this IServiceCollection services)
        {
            services.AddTransient<IBocCurrencyConverterSvc, BocCurrencyConverterSvc>();
            return services;
        }

    }
}
