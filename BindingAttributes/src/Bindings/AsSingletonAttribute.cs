using System;

using Microsoft.Extensions.DependencyInjection;


namespace BindingAttributes {

    public class AsSingletonAttribute : BindingAttribute {

        public AsSingletonAttribute() : base(ServiceLifetime.Singleton) { }

        public AsSingletonAttribute(Type serviceType) : base(ServiceLifetime.Singleton, serviceType) { }

    }

}