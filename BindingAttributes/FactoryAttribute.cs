using System;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;


namespace BindingAttributes {

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class FactoryAttribute : Attribute {

        protected static MethodInfo binder;

        static FactoryAttribute() {
            binder = FindBinder();
        }

        protected static MethodInfo FindBinder() {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
            var searchType = typeof(ServiceCollectionServiceExtensions);
            var methods = searchType.GetMethods(flags);

            return (from m in methods
                    where m.Name == "AddSingleton" &&
                          m.GetGenericArguments().Length == 1 &&
                          m.GetParameters().Length == 2 &&
                          m.GetGenericArguments()[0] != m.GetParameters()[1].ParameterType
                    select m).Single();
        }

        public void Bind(IServiceCollection services, MethodInfo target) {
            var closedBinder = binder.MakeGenericMethod(target.ReturnType);
            var targetDelegate = Delegate.CreateDelegate(closedBinder.GetParameters()[1].ParameterType, target);
            closedBinder.Invoke(null, new object[] {services, targetDelegate});
        }

        public static void ConfigureFactories(IServiceCollection services) {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var type in assembly.GetTypes()) {
                    foreach (var method in type.GetRuntimeMethods()) {
                        if (!method.IsStatic) {
                            continue;
                        }

                        var methodAttrs = method.GetCustomAttributes(typeof(FactoryAttribute));

                        if (methodAttrs.Count() == 0) {
                            continue;
                        }

                        foreach (FactoryAttribute attr in methodAttrs) {
                            attr.Bind(services, method);
                        }
                    }
                }
            }
        }

    }

}