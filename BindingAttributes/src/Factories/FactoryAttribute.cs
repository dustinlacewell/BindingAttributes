using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BindingAttributes.Extensions;

using Microsoft.Extensions.DependencyInjection;


namespace BindingAttributes {

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class FactoryAttribute : Attribute {

        private static MethodInfo _asSingleton;
        private static MethodInfo _asScoped;
        private static MethodInfo _asTransient;

        private readonly ServiceLifetime _serviceLifetime;

        static FactoryAttribute() {
            _asSingleton = BinderFinder.Find(BinderMethod.Singleton);
            _asScoped = BinderFinder.Find(BinderMethod.Scoped);
            _asTransient = BinderFinder.Find(BinderMethod.Transient);
        }

        public FactoryAttribute() {
            _serviceLifetime = ServiceLifetime.Transient;
        }

        public FactoryAttribute(ServiceLifetime serviceLifetime) {
            _serviceLifetime = serviceLifetime;
        }

        private MethodInfo GetBinder() {
            switch (_serviceLifetime) {
                case ServiceLifetime.Scoped:
                    return _asScoped;

                case ServiceLifetime.Singleton:
                    return _asSingleton;

                default:
                    return _asTransient;
            }
        }

        private void Bind(IServiceCollection services, MethodInfo target) {
            var closure = new Func<IServiceProvider, object>(s => s.InvokeWithServices(target));

            services.Add(new ServiceDescriptor(target.ReturnType, closure, _serviceLifetime));

        }

        public static void ConfigureFactories(IServiceCollection services, IEnumerable<Assembly> assemblies=null) {
            if (assemblies == null) assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) {
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