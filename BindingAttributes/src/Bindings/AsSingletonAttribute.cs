using System;


namespace BindingAttributes {

    public class AsSingletonAttribute : BindingAttribute {

        public AsSingletonAttribute() : base(BindType.Singleton) { }

        public AsSingletonAttribute(Type serviceType) : base(BindType.Singleton, serviceType) { }

    }

}