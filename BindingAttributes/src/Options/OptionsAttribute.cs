using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace BindingAttributes {

    [AttributeUsage(AttributeTargets.Class)]
    public class OptionsAttribute : Attribute {

        public string Section { get; }

        public OptionsAttribute(string section = null) {
            Section = section;
        }

        private static MethodInfo GetAddOptionsMethod(Type optionsType) {
            return GetGenericMethod(typeof(OptionsServiceCollectionExtensions),
                                    optionsType,
                                    "AddOptions");
        }

        private static MethodInfo GetBindMethod(Type optionsType) {
            return GetGenericMethod(typeof(OptionsBuilderConfigurationExtensions),
                                    optionsType,
                                    "Bind", 2);
        }

        private static MethodInfo GetValidateMethod(Type optionsType) {
            return GetGenericMethod(typeof(BindingAttributesOptionsBuilderExtensions),
                                    optionsType,
                                    "ValidateOptionsAnnotations");
        }

        private static MethodInfo GetGenericMethod(Type sourceType, Type optionsType, string name, int parameters = 1) {

            return sourceType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                             .Where(m => m.Name == name)
                             .Where(m => m.GetParameters().Length == parameters)
                             .First(m => m.IsGenericMethod)
                             .MakeGenericMethod(optionsType);

        }

        public void AddOption(IServiceCollection services, IConfiguration configuration, Type optionType) {
            var addOptionsMethod = GetAddOptionsMethod(optionType);
            var bindMethod = GetBindMethod(optionType);
            var validateMethod = GetValidateMethod(optionType);

            var builder = addOptionsMethod.Invoke(null, new object[] {services});
            builder = bindMethod.Invoke(null, new[] {builder, configuration});
            validateMethod.Invoke(null, new[] {builder});
        }

        public static void ConfigureOptions(IServiceCollection services,
                                            IConfiguration configuration,
                                            IEnumerable<Assembly> assemblies = null) {

            assemblies = assemblies ?? AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies) {
                foreach (var type in assembly.GetTypes()) {
                    services.AddOptions<BindingAttribute>().Bind(configuration);

                    var attrs = type.GetCustomAttributes<OptionsAttribute>();

                    if (!attrs.Any()) {
                        continue;
                    }

                    var attr = attrs.First();
                    var section = attr.Section != null ? configuration.GetSection(attr.Section) : configuration;
                    attr.AddOption(services, section, type);
                }
            }
        }

    }

}