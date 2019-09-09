using System;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Xunit.Abstractions;


namespace BindingAttributes.Tests {

    public class BindingAttributeMethodTests : BindingAttributeTestsBase {

        public BindingAttributeMethodTests(ITestOutputHelper output) : base(output) { }


        [Theory]
        [InlineData(typeof(ImplicitSingletonFactoryService), typeof(ImplicitSingletonFactoryService), ServiceLifetime.Singleton)]
        [InlineData(typeof(IImplicitSingletonFactoryService), typeof(ImplicitSingletonFactoryService), ServiceLifetime.Singleton)]
        [InlineData(typeof(ExplicitSingletonFactoryService), typeof(ExplicitSingletonFactoryService), ServiceLifetime.Singleton)]
        [InlineData(typeof(IExplicitSingletonFactoryService), typeof(ExplicitSingletonFactoryService), ServiceLifetime.Singleton)]
        [InlineData(typeof(ExplicitScopedFactoryService), typeof(ExplicitScopedFactoryService), ServiceLifetime.Scoped)]
        [InlineData(typeof(IExplicitScopedFactoryService), typeof(ExplicitScopedFactoryService), ServiceLifetime.Scoped)]
        [InlineData(typeof(ExplicitTransientFactoryService), typeof(ExplicitTransientFactoryService), ServiceLifetime.Transient)]
        [InlineData(typeof(IExplicitTransientFactoryService), typeof(ExplicitTransientFactoryService), ServiceLifetime.Transient)]
        [InlineData(typeof(HelperSingletonFactoryService), typeof(HelperSingletonFactoryService), ServiceLifetime.Singleton)]
        [InlineData(typeof(IHelperSingletonFactoryService), typeof(HelperSingletonFactoryService), ServiceLifetime.Singleton)]
        [InlineData(typeof(HelperScopedFactoryService), typeof(HelperScopedFactoryService), ServiceLifetime.Scoped)]
        [InlineData(typeof(IHelperScopedFactoryService), typeof(HelperScopedFactoryService), ServiceLifetime.Scoped)]
        [InlineData(typeof(HelperTransientFactoryService), typeof(HelperTransientFactoryService), ServiceLifetime.Transient)]
        [InlineData(typeof(IHelperTransientFactoryService), typeof(HelperTransientFactoryService), ServiceLifetime.Transient)]
        public void CanBindMethods(Type serviceType, Type implementationType, ServiceLifetime lifetime) {
            var serviceDescriptor = FindBoundService(serviceType);
            Assert.NotNull(serviceDescriptor);
            Assert.Equal(lifetime, serviceDescriptor.Lifetime);
            Assert.Equal(serviceType, serviceDescriptor.ServiceType);
            Assert.IsType<Func<IServiceProvider, Object>>(serviceDescriptor.ImplementationFactory);
            Assert.NotNull(serviceDescriptor.ImplementationFactory);
        }

        [Theory]
        [InlineData(typeof(ImplicitSingletonFactoryService), typeof(ImplicitSingletonFactoryService))]
        [InlineData(typeof(IImplicitSingletonFactoryService), typeof(ImplicitSingletonFactoryService))]
        [InlineData(typeof(ExplicitSingletonFactoryService), typeof(ExplicitSingletonFactoryService))]
        [InlineData(typeof(IExplicitSingletonFactoryService), typeof(ExplicitSingletonFactoryService))]
        [InlineData(typeof(ExplicitScopedFactoryService), typeof(ExplicitScopedFactoryService))]
        [InlineData(typeof(IExplicitScopedFactoryService), typeof(ExplicitScopedFactoryService))]
        [InlineData(typeof(ExplicitTransientFactoryService), typeof(ExplicitTransientFactoryService))]
        [InlineData(typeof(IExplicitTransientFactoryService), typeof(ExplicitTransientFactoryService))]
        [InlineData(typeof(HelperSingletonFactoryService), typeof(HelperSingletonFactoryService))]
        [InlineData(typeof(IHelperSingletonFactoryService), typeof(HelperSingletonFactoryService))]
        [InlineData(typeof(HelperScopedFactoryService), typeof(HelperScopedFactoryService))]
        [InlineData(typeof(IHelperScopedFactoryService), typeof(HelperScopedFactoryService))]
        [InlineData(typeof(HelperTransientFactoryService), typeof(HelperTransientFactoryService))]
        [InlineData(typeof(IHelperTransientFactoryService), typeof(HelperTransientFactoryService))]
        public void CanInjectMethodsFactories(Type serviceType, Type implementationType) {
            var instance = _provider.GetRequiredService(serviceType);
            var instanceType = instance.GetType();
            Assert.True(serviceType.IsAssignableFrom(instanceType));
        }

    }
    
}