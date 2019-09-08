using System;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Xunit;


namespace BindingAttributes.Tests {

    public class BindingAttributeTests {

        private readonly IServiceCollection _services;

        public BindingAttributeTests() {
            _services = new ServiceCollection();
            
            BindingAttribute.ConfigureBindings(_services, new[] {Assembly.GetExecutingAssembly()});

        }

        [Fact]
        public void CanInstantiate() { }

        [Theory]
        [InlineData(typeof(ServiceA))]
        [InlineData(typeof(ServiceB))]
        [InlineData(typeof(ServiceC))]
        [InlineData(typeof(ServiceD))]
        [InlineData(typeof(ServiceE))]
        [InlineData(typeof(ServiceF))]
        public void CanBindServiceDirectly(Type serviceType) {
            var serviceDescriptor = FindBoundService(serviceType);
            
            Assert.NotNull(serviceDescriptor);
            Assert.Equal(serviceType, serviceDescriptor.ImplementationType);
        }

        [Theory]
        [InlineData(typeof(IServiceA), typeof(ServiceA))]
        [InlineData(typeof(IServiceB), typeof(ServiceB))]
        [InlineData(typeof(IServiceC), typeof(ServiceC))]
        [InlineData(typeof(IServiceD), typeof(ServiceD))]
        [InlineData(typeof(IServiceE), typeof(ServiceE))]
        [InlineData(typeof(IServiceF), typeof(ServiceF))]
        public void CanBindServiceImplementation(Type serviceType, Type implementationType) {
            var serviceDescriptor = FindBoundService(serviceType);
            
            Assert.NotNull(serviceDescriptor);
            Assert.Equal(implementationType, serviceDescriptor.ImplementationType);
        }

        [Theory]
        [InlineData(typeof(ServiceA), ServiceLifetime.Singleton)]
        [InlineData(typeof(ServiceB), ServiceLifetime.Singleton)]
        [InlineData(typeof(ServiceC), ServiceLifetime.Scoped)]
        [InlineData(typeof(ServiceD), ServiceLifetime.Scoped)]
        [InlineData(typeof(ServiceE), ServiceLifetime.Transient)]
        [InlineData(typeof(ServiceF), ServiceLifetime.Transient)]
        public void CanBindProperLifetime(Type serviceType, ServiceLifetime serviceLifetime) {
            var serviceDescriptor = FindBoundService(serviceType);
            
            Assert.NotNull(serviceDescriptor);
            Assert.Equal(serviceLifetime, serviceDescriptor.Lifetime);
        }

        private ServiceDescriptor FindBoundService(Type serviceType) {
            return _services.FirstOrDefault(serviceDescriptor => serviceDescriptor.ServiceType == serviceType);

        }

    }

    [Binding(ServiceLifetime.Singleton)]
    [Binding(ServiceLifetime.Singleton, typeof(IServiceA))]
    internal class ServiceA : IServiceA { }

    [AsSingleton]
    [AsSingleton(typeof(IServiceB))]
    internal class ServiceB : IServiceB { }

    [Binding(ServiceLifetime.Scoped)]
    [Binding(ServiceLifetime.Scoped, typeof(IServiceC))]
    internal class ServiceC : IServiceC { }

    [AsScoped]
    [AsScoped(typeof(IServiceD))]
    internal class ServiceD : IServiceD { }

    [Binding(ServiceLifetime.Transient)]
    [Binding(ServiceLifetime.Transient, typeof(IServiceE))]
    internal class ServiceE : IServiceE { }

    [AsTransient]
    [AsTransient(typeof(IServiceF))]
    internal class ServiceF : IServiceF { }

    internal interface IServiceA { }

    internal interface IServiceB { }

    internal interface IServiceC { }

    internal interface IServiceD { }

    internal interface IServiceE { }

    internal interface IServiceF { }

}