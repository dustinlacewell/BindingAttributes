using System;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;


namespace BindingAttributes.Extensions {

    public static class ServiceProviderExtensions {

        public static object InvokeWithServices(this IServiceProvider sp, MethodInfo method, params object[] parameters) {
            if (method.GetParameters().Length < parameters.Length) {
                throw new InvalidOperationException(
                    $"Cannot invoke method '{method.Name}' as it only takes {method.GetParameters().Length} params, but {parameters.Length} were given.");
            }

            var parameterInfos = method.GetParameters()
                                     .Skip(parameters.Length)
                                     .ToArray();

            var services = new object[parameterInfos.Length];

            foreach (var pInfo in parameterInfos) {
                var resolvedService = sp.GetRequiredService(pInfo.ParameterType);
                services[pInfo.Position - parameters.Length] = resolvedService;
            }

            var allParams = parameters.Concat(services).ToArray();

            return method.Invoke(null, allParams);
        }

    }

}