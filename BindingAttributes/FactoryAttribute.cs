using System;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;


namespace BindingAttributes {

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class FactoryAttribute : Attribute {

        private static MethodInfo _asSingleton;
        private static MethodInfo _asScoped;
        private static MethodInfo _asTransient;

        private BindType _bindType;

        static FactoryAttribute() {
            _asSingleton = BinderFinder.Find(BinderMethod.Singleton);
            _asScoped = BinderFinder.Find(BinderMethod.Scoped);
            _asTransient = BinderFinder.Find(BinderMethod.Transient);
        }

        public FactoryAttribute() {
            _bindType = BindType.Transient;
        }

        public FactoryAttribute(BindType bindType) {
            _bindType = bindType;
        }

        private MethodInfo GetBinder() {
            switch (_bindType) {
                case BindType.Scoped:
                    return _asScoped;

                case BindType.Singleton:
                    return _asSingleton;

                default:
                    return _asTransient;
            }
        }

        private void Bind(IServiceCollection services, MethodInfo target) {
            var openBinder = GetBinder();            
            var closedBinder = openBinder.MakeGenericMethod(target.ReturnType);
            var targetDelegate = Delegate.CreateDelegate(closedBinder.GetParameters()[1].ParameterType, target);
            closedBinder.Invoke(null, new object[] {services, targetDelegate});
        }

        public static void ConfigureFactories(IServiceCollection services) {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var type in assembly.GetTypes()) {
                    foreach (var method in type.GetRuntimeMethods()) {
                        if (!method.IsStatic) {
                            continue;
                        }

                        var methodAttrs = method.GetCustomAttributes(typeof(FactoryAttribute));

                        if (methodAttrs.Count() == 0) {
                            continue;
                        }

                        foreach (FactoryAttribute attr in methodAttrs) {
                            attr.Bind(services, method);
                        }
                    }
                }
            }
        }

    }

}