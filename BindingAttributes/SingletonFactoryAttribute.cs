namespace BindingAttributes {

    public class SingletonFactoryAttribute : FactoryAttribute {

        public SingletonFactoryAttribute() : base(BindType.Singleton) { }

    }

}