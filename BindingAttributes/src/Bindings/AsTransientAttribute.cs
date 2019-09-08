using System;

using Microsoft.Extensions.DependencyInjection;


namespace BindingAttributes {

    public class AsTransientAttribute : BindingAttribute {

        public AsTransientAttribute() : base(ServiceLifetime.Transient) { }

        public AsTransientAttribute(Type serviceType) : base(ServiceLifetime.Transient, serviceType) { }

    }

}