using System;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using MethodInfo = System.Reflection.MethodInfo;


namespace BindingAttributes {

    public enum BinderMethod {

        Transient,
        Scoped,
        Singleton

    }
    
    public class BinderFinder {

        private static string NameForMethod(BinderMethod method) {
            switch (method) {
                case BinderMethod.Transient: {
                    return "AddTransient";
                }

                case BinderMethod.Scoped: {
                    return "AddScoped";
                }

                case BinderMethod.Singleton: {
                    return "AddSingleton";
                }

                default: {
                    return "AddSingleton";
                }
            }
        }

        public static MethodInfo Find(BinderMethod binder) {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
            var searchType = typeof(ServiceCollectionServiceExtensions);
            var methods = searchType.GetMethods(flags);
            var methodName = NameForMethod(binder);


            return (from m in methods
                    where m.Name == methodName &&
                          m.GetGenericArguments().Length == 1 &&
                          m.GetParameters().Length == 2 &&
                          m.GetGenericArguments()[0] != m.GetParameters()[1].ParameterType
                    select m).Single();
        }

    }

}