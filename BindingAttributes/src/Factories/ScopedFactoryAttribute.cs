using Microsoft.Extensions.DependencyInjection;


namespace BindingAttributes {

    public class ScopedFactoryAttribute : FactoryAttribute {

        public ScopedFactoryAttribute() : base(ServiceLifetime.Scoped) { }

    }

}