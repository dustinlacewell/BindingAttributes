using Microsoft.Extensions.DependencyInjection;


namespace BindingAttributes {

    public class TransientFactoryAttribute : FactoryAttribute {

        public TransientFactoryAttribute() : base(ServiceLifetime.Transient) { }

    }

}