using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;


namespace BindingAttributes {

    using Binder = Action<IServiceCollection>;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class BindingAttribute : Attribute {

        protected ServiceLifetime _serviceLifetime;
        protected Type _serviceType;

        public BindingAttribute(ServiceLifetime serviceLifetime, Type serviceType) {
            _serviceLifetime = serviceLifetime;
            _serviceType = serviceType;
        }

        public BindingAttribute(ServiceLifetime serviceLifetime) {
            _serviceLifetime = serviceLifetime;
        }

        public BindingAttribute(Type serviceType) {
            _serviceLifetime = ServiceLifetime.Transient;
            _serviceType = serviceType;
        }

        public BindingAttribute() {
            _serviceLifetime = ServiceLifetime.Transient;
        }

        public void Bind(IServiceCollection services, Type serviceType, Type implementationType) {
            switch (_serviceLifetime) {
                case ServiceLifetime.Singleton:
                    services.AddSingleton(serviceType, implementationType);
                    break;

                case ServiceLifetime.Scoped:
                    services.AddScoped(serviceType, implementationType);
                    break;

                case ServiceLifetime.Transient:
                    services.AddTransient(serviceType, implementationType);
                    break;
            }
        }

        public void BindWith(IServiceCollection services, Type serviceType, MethodInfo handler) {
            var closure = new Func<IServiceProvider, object>(s => { return handler.Invoke(null, new[] {s}); });

            switch (_serviceLifetime) {
                case ServiceLifetime.Singleton:
                    services.AddSingleton(serviceType, closure);
                    break;

                case ServiceLifetime.Scoped:
                    services.AddScoped(serviceType, closure);
                    break;

                case ServiceLifetime.Transient:
                    services.AddTransient(serviceType, closure);
                    break;
            }
        }

        public void Execute(IServiceCollection services, Type implementationType) {
            if (_serviceType == null) {
                Bind(services, implementationType, implementationType);
            } else {
                Bind(services, _serviceType, implementationType);
            }
        }

        public void ExecuteWith(IServiceCollection services, Type implementationType, MethodInfo handler) {
            if (_serviceType == null) {
                BindWith(services, implementationType, handler);
            } else {
                BindWith(services, _serviceType, handler);
            }
        }

        public static void ConfigureBindings(IServiceCollection services, IEnumerable<Assembly> assemblies=null) {
            
            if (assemblies == null) assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) {
                foreach (var implementationType in assembly.GetTypes()) {
                    var classAttrs = implementationType.GetCustomAttributes(typeof(BindingAttribute));

                    foreach (BindingAttribute attr in classAttrs) {
                        attr.Execute(services, implementationType);
                    }

                    foreach (var method in implementationType.GetRuntimeMethods()) {
                        if (!method.IsStatic) {
                            continue;
                        }

                        var methodAttrs = method.GetCustomAttributes(typeof(BindingAttribute));

                        if (methodAttrs.Count() == 0) {
                            continue;
                        }

                        foreach (BindingAttribute attr in methodAttrs) {
                            attr.ExecuteWith(services, implementationType, method);
                        }
                    }
                }
            }
        }

    }

}
