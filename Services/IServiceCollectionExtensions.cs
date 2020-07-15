using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

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
