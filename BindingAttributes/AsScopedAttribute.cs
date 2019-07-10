using System;


namespace BindingAttributes {

    public class AsScopedAttribute : BindingAttribute {

        public AsScopedAttribute() : base(BindType.Scoped) { }

        public AsScopedAttribute(Type serviceType) : base(BindType.Scoped, serviceType) { }

    }

}