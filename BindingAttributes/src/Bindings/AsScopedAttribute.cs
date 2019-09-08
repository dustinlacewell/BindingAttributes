using System;

using Microsoft.Extensions.DependencyInjection;


namespace BindingAttributes {

    public class AsScopedAttribute : BindingAttribute {

        public AsScopedAttribute() : base(ServiceLifetime.Scoped) { }

        public AsScopedAttribute(Type serviceType) : base(ServiceLifetime.Scoped, serviceType) { }

    }

}