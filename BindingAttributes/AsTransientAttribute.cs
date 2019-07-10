using System;


namespace BindingAttributes {

    public class AsTransientAttribute : BindingAttribute {

        public AsTransientAttribute() : base(BindType.Transient) { }

        public AsTransientAttribute(Type serviceType) : base(BindType.Transient, serviceType) { }

    }

}