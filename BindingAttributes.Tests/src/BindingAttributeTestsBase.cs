using System;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;


namespace BindingAttributes.Tests {
    public class BindingAttributeTestsBase : XunitLoggingBase {

        protected readonly IServiceCollection _services;
        protected readonly IServiceProvider _provider;

        public BindingAttributeTestsBase(ITestOutputHelper output) : base(output) {
            _services = new ServiceCollection()
                .AddSingleton<IFakeDependency>(new FakeDependency());

            BindingAttribute.ConfigureBindings(_services, new[] {Assembly.GetExecutingAssembly()});

            _provider = _services.BuildServiceProvider();

        }

        protected ServiceDescriptor FindBoundService(Type serviceType) {
            return _services.FirstOrDefault(serviceDescriptor => serviceDescriptor.ServiceType == serviceType);
        }

    }

}