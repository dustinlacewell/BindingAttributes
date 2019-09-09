using System;

using Microsoft.Extensions.DependencyInjection;


namespace BindingAttributes.Tests {

    internal interface IFakeDependency { }

    internal class FakeDependency : IFakeDependency { }

    [Binding(ServiceLifetime.Singleton)]
    [Binding(ServiceLifetime.Singleton, typeof(IExplicitSingletonService))]
    internal class ExplicitSingletonExplicitSingletonService : IExplicitSingletonService { }

    [AsSingleton]
    [AsSingleton(typeof(IHelperSingletonService))]
    internal class HelperSingletonHelperSingletonService : IHelperSingletonService { }

    [Binding(ServiceLifetime.Scoped)]
    [Binding(ServiceLifetime.Scoped, typeof(IExplicitScopedService))]
    internal class ExplicitScopedExplicitScopedService : IExplicitScopedService { }

    [AsScoped]
    [AsScoped(typeof(IHelperScopedService))]
    internal class HelperScopedHelperScopedService : IHelperScopedService { }

    [Binding(ServiceLifetime.Transient)]
    [Binding(ServiceLifetime.Transient, typeof(IExplicitTransientService))]
    internal class ExplicitTransientExplicitTransientService : IExplicitTransientService { }

    [AsTransient]
    [AsTransient(typeof(IHelperTransientService))]
    internal class HelperTransientHelperTransientService : IHelperTransientService { }

    internal class FactoryServiceBase<T> where T : FactoryServiceBase<T> {

        public IFakeDependency FakeDependency { get; }

        protected FactoryServiceBase(IFakeDependency fakeDependency) {
            FakeDependency = fakeDependency;
        }

        protected static T Factory(IServiceProvider sp, IFakeDependency fakeDependency) {
            return (T) Activator.CreateInstance(typeof(T), fakeDependency);
        }

    }

    internal class ImplicitSingletonFactoryService : FactoryServiceBase<ImplicitSingletonFactoryService>,
                                                     IImplicitSingletonFactoryService {

        public ImplicitSingletonFactoryService(IFakeDependency fakeDependency) : base(fakeDependency) { }

        [Binding]
        [Binding(typeof(IImplicitSingletonFactoryService))]
        public static ImplicitSingletonFactoryService
            FactoryMethod(IServiceProvider sp, IFakeDependency fakeDependency) {
            return Factory(sp, fakeDependency);
        }

    }

    internal class ExplicitSingletonFactoryService : FactoryServiceBase<ExplicitSingletonFactoryService>,
                                                     IExplicitSingletonFactoryService {

        public ExplicitSingletonFactoryService(IFakeDependency fakeDependency) : base(fakeDependency) { }

        [Binding(ServiceLifetime.Singleton)]
        [Binding(ServiceLifetime.Singleton, typeof(IExplicitSingletonFactoryService))]
        public static ExplicitSingletonFactoryService FactoryMethod(
            IServiceProvider sp, IFakeDependency fakeDependency) {
            return Factory(sp, fakeDependency);
        }

    }

    internal class HelperSingletonFactoryService : FactoryServiceBase<HelperSingletonFactoryService>,
                                                   IHelperSingletonFactoryService {

        public HelperSingletonFactoryService(IFakeDependency fakeDependency) : base(fakeDependency) { }

        [AsSingleton]
        [AsSingleton(typeof(IHelperSingletonFactoryService))]
        public static HelperSingletonFactoryService FactoryMethod(
            IServiceProvider sp, IFakeDependency fakeDependency) {
            return Factory(sp, fakeDependency);
        }

    }

    internal class ExplicitScopedFactoryService : FactoryServiceBase<ExplicitScopedFactoryService>,
                                                  IExplicitScopedFactoryService {

        public ExplicitScopedFactoryService(IFakeDependency fakeDependency) : base(fakeDependency) { }

        [Binding(ServiceLifetime.Scoped)]
        [Binding(ServiceLifetime.Scoped, typeof(IExplicitScopedFactoryService))]
        public static ExplicitScopedFactoryService FactoryMethod(
            IServiceProvider sp, IFakeDependency fakeDependency) {
            return Factory(sp, fakeDependency);
        }

    }

    internal class HelperScopedFactoryService : FactoryServiceBase<HelperScopedFactoryService>,
                                                IHelperScopedFactoryService {

        public HelperScopedFactoryService(IFakeDependency fakeDependency) : base(fakeDependency) { }

        [AsScoped]
        [AsScoped(typeof(IHelperScopedFactoryService))]
        public static HelperScopedFactoryService FactoryMethod(
            IServiceProvider sp, IFakeDependency fakeDependency) {
            return Factory(sp, fakeDependency);
        }

    }

    internal class ExplicitTransientFactoryService : FactoryServiceBase<ExplicitTransientFactoryService>,
                                                     IExplicitTransientFactoryService {

        public ExplicitTransientFactoryService(IFakeDependency fakeDependency) : base(fakeDependency) { }

        [Binding(ServiceLifetime.Transient)]
        [Binding(ServiceLifetime.Transient, typeof(IExplicitTransientFactoryService))]
        public static ExplicitTransientFactoryService FactoryMethod(
            IServiceProvider sp, IFakeDependency fakeDependency) {
            return Factory(sp, fakeDependency);
        }

    }

    internal class HelperTransientFactoryService : FactoryServiceBase<HelperTransientFactoryService>,
                                                   IHelperTransientFactoryService {

        public HelperTransientFactoryService(IFakeDependency fakeDependency) : base(fakeDependency) { }

        [AsTransient]
        [AsTransient(typeof(IHelperTransientFactoryService))]
        public static HelperTransientFactoryService FactoryMethod(
            IServiceProvider sp, IFakeDependency fakeDependency) {
            return Factory(sp, fakeDependency);
        }

    }

    internal interface IExplicitSingletonService { }

    internal interface IHelperSingletonService { }

    internal interface IExplicitScopedService { }

    internal interface IHelperScopedService { }

    internal interface IExplicitTransientService { }

    internal interface IHelperTransientService { }

    internal interface IImplicitSingletonFactoryService { }

    internal interface IExplicitSingletonFactoryService { }

    internal interface IHelperSingletonFactoryService { }

    internal interface IExplicitScopedFactoryService { }

    internal interface IHelperScopedFactoryService { }

    internal interface IExplicitTransientFactoryService { }

    internal interface IHelperTransientFactoryService { }

}