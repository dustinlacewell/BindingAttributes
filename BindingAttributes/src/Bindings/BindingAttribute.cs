using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BindingAttributes.Extensions;

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
            services.Add(new ServiceDescriptor(serviceType, implementationType, _serviceLifetime));
        }

        public void BindWith(IServiceCollection services, Type serviceType, MethodInfo handler) {
            var closure = new Func<IServiceProvider, object>(s => s.InvokeWithServices(handler));
            services.Add(new ServiceDescriptor(serviceType, closure, _serviceLifetime));
        }

        public void Execute(IServiceCollection services, Type implementationType) {
            Bind(services, _serviceType ?? implementationType, implementationType);
        }

        public void ExecuteWith(IServiceCollection services, MethodInfo handler) {
            BindWith(services, _serviceType ?? handler.ReturnType, handler);
        }

        public static void ConfigureBindings(IServiceCollection services, IEnumerable<Assembly> assemblies = null) {

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

                        if (!methodAttrs.Any()) {
                            continue;
                        }
                        
                        foreach (BindingAttribute attr in methodAttrs) {
                            
                            attr.ExecuteWith(services, method);
                        }
                    }
                }
            }
        }

    }

}