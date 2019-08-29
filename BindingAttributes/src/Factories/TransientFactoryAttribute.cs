namespace BindingAttributes {

    public class TransientFactoryAttribute : FactoryAttribute {

        public TransientFactoryAttribute() : base(BindType.Transient) { }

    }

}