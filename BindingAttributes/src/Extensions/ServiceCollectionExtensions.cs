using System.Collections.Generic;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace BindingAttributes.Extensions {

    public static class ServiceCollectionExtensions {

        public static IServiceCollection AddBindings(this IServiceCollection services,
                                                     IEnumerable<Assembly> assemblies = null) {
            BindingAttribute.ConfigureBindings(services, assemblies);
            return services;
        }

        public static IServiceCollection AddOptions(this IServiceCollection services,
                                                    IConfiguration configuration,
                                                    IEnumerable<Assembly> assemblies = null) {
            OptionsAttribute.ConfigureOptions(services, configuration, assemblies);
            return services;
        }
        
    }

}