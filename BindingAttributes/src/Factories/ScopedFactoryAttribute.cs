namespace BindingAttributes {

    public class ScopedFactoryAttribute : FactoryAttribute {

        public ScopedFactoryAttribute() : base(BindType.Scoped) { }

    }

}