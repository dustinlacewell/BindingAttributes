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

        protected BindType _bindType;
        protected Type _serviceType;

        public BindingAttribute(BindType bindType, Type serviceType) {
            _bindType = bindType;
            _serviceType = serviceType;
        }

        public BindingAttribute(BindType bindType) {
            _bindType = bindType;
        }

        public BindingAttribute(Type serviceType) {
            _bindType = BindType.Transient;
            _serviceType = serviceType;
        }

        public BindingAttribute() {
            _bindType = BindType.Transient;
        }

        public void Bind(IServiceCollection services, Type serviceType, Type implementationType) {
            switch (_bindType) {
                case BindType.Singleton:
                    services.AddSingleton(serviceType, implementationType);
                    break;

                case BindType.Scoped:
                    services.AddScoped(serviceType, implementationType);
                    break;

                case BindType.Transient:
                    services.AddTransient(serviceType, implementationType);
                    break;
            }
        }

        public void BindWith(IServiceCollection services, Type serviceType, MethodInfo handler) {
            var closure = new Func<IServiceProvider, object>(s => handler.Invoke(null, new[] {s}));

            switch (_bindType) {
                case BindType.Singleton:
                    services.AddSingleton(serviceType, closure);
                    break;

                case BindType.Scoped:
                    services.AddScoped(serviceType, closure);
                    break;

                case BindType.Transient:
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
