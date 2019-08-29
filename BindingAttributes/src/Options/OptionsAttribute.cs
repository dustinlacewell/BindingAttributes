

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace BindingAttributes {

    [AttributeUsage(AttributeTargets.Class)]
    public class OptionsAttribute : Attribute {

        public static MethodInfo GetConfigureMethod(IServiceCollection services,
                                                    Type optionType) {
            var serviceType = services.GetType();
            var serviceMethods = serviceType.GetMethods(BindingFlags.Public | 
                                                        BindingFlags.Instance);

            var optionsMethods = serviceMethods.Where(m => m.Name == "Configure");
            var genericMethods = optionsMethods.Where(m => m.ContainsGenericParameters);
            var openMethod = genericMethods.First();
            return openMethod.MakeGenericMethod(optionType);
            
        }

        public static void AddOption(IServiceCollection services, 
                                     Type optionType, 
                                     IConfiguration configuration) {
            var configureMethod = GetConfigureMethod(services, optionType);
            configureMethod.Invoke(services, new object[] {configuration});
        }

        public static void ConfigureOptions(IServiceCollection services,
                                            IConfiguration configuration,
                                            IEnumerable<Assembly> assemblies = null) {

            assemblies = assemblies ?? AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies) {
                foreach (var implementationType in assembly.GetTypes()) {
                    var classAttrs = implementationType.GetCustomAttributes(typeof(OptionsAttribute));

                    foreach (OptionsAttribute attr in classAttrs) {
                        AddOption(services, implementationType, configuration);
                    }
                }
            }
        }
        
    }

}