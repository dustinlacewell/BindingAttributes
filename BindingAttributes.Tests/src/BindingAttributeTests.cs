using System;
using System.Linq;
using System.Reflection;

using BindingAttributes.Extensions;

using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;
using Xunit;

namespace BindingAttributes.Tests {

 
 
    public class BindingAttributeTests : BindingAttributeTestsBase {

        public BindingAttributeTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void CanInstantiate() { }

        [Theory]
        [InlineData(typeof(ExplicitSingletonExplicitSingletonService))]
        [InlineData(typeof(HelperSingletonHelperSingletonService))]
        [InlineData(typeof(ExplicitScopedExplicitScopedService))]
        [InlineData(typeof(HelperScopedHelperScopedService))]
        [InlineData(typeof(ExplicitTransientExplicitTransientService))]
        [InlineData(typeof(HelperTransientHelperTransientService))]
        public void CanBindServiceDirectly(Type serviceType) {
            var serviceDescriptor = FindBoundService(serviceType);

            Assert.NotNull(serviceDescriptor);
            Assert.Equal(serviceType, serviceDescriptor.ImplementationType);
        }

        [Theory]
        [InlineData(typeof(IExplicitSingletonService), typeof(ExplicitSingletonExplicitSingletonService))]
        [InlineData(typeof(IHelperSingletonService), typeof(HelperSingletonHelperSingletonService))]
        [InlineData(typeof(IExplicitScopedService), typeof(ExplicitScopedExplicitScopedService))]
        [InlineData(typeof(IHelperScopedService), typeof(HelperScopedHelperScopedService))]
        [InlineData(typeof(IExplicitTransientService), typeof(ExplicitTransientExplicitTransientService))]
        [InlineData(typeof(IHelperTransientService), typeof(HelperTransientHelperTransientService))]
        public void CanBindServiceImplementation(Type serviceType, Type implementationType) {
            var serviceDescriptor = FindBoundService(serviceType);

            Assert.NotNull(serviceDescriptor);
            Assert.Equal(implementationType, serviceDescriptor.ImplementationType);
        }

        [Theory]
        [InlineData(typeof(ExplicitSingletonExplicitSingletonService), ServiceLifetime.Singleton)]
        [InlineData(typeof(HelperSingletonHelperSingletonService), ServiceLifetime.Singleton)]
        [InlineData(typeof(ExplicitScopedExplicitScopedService), ServiceLifetime.Scoped)]
        [InlineData(typeof(HelperScopedHelperScopedService), ServiceLifetime.Scoped)]
        [InlineData(typeof(ExplicitTransientExplicitTransientService), ServiceLifetime.Transient)]
        [InlineData(typeof(HelperTransientHelperTransientService), ServiceLifetime.Transient)]
        public void CanBindProperLifetime(Type serviceType, ServiceLifetime serviceLifetime) {
            var serviceDescriptor = FindBoundService(serviceType);

            Assert.NotNull(serviceDescriptor);
            Assert.Equal(serviceLifetime, serviceDescriptor.Lifetime);
        }
    }


   
}