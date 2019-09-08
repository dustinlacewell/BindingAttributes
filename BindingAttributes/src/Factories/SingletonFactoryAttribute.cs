using Microsoft.Extensions.DependencyInjection;


namespace BindingAttributes {

    public class SingletonFactoryAttribute : FactoryAttribute {

        public SingletonFactoryAttribute() : base(ServiceLifetime.Singleton) { }

    }

}