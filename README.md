# BindingAttributes

This library implements a number of helpful attributes for use with the `Microsoft.Extensions.DependencyInjection` container.

A short guide: [Bindings and Factories](https://github.com/dustinlacewell/BindingAttributes#bindings-and-factories)

## Installation

There is no Nuget package available yet.

### Installing with Paket

Add the following to your `paket.dependencies`:

    git https://github.com/dustinlacewell/BindingAttributes.git master build: "build.sh", OS: mono, Packages: /BindingAttributes/nupkg
    git https://github.com/dustinlacewell/BindingAttributes.git master build: "build.bat", OS: windows, Packages: /BindingAttributes/nupkg
    nuget BindingAttributes

Add the following to the `paket.references` of any project that uses BindingAttributes:

    BindingAttributes

Finally update your packages:

    paket install


# Feature Activation

For any of the attributes within this package to work, you must configure your `IServiceCollection` appropriately:

```cs
BindingAttribute.ConfigureBindings(services);
FactoryAttribute.ConfigureFactories(services);
OptionsAttribute.ConfigureOptions(services, configuration);
```


## Attribute Overview

### BindTypes

By default the binding scopes are Singleton. Specify a particular scope by passing a `BindType` enumeration:

- *BindType*.**Transient**
- *BindType*.**Scoped** *BindType*.**Singleton**

### Class Attributes

**[Binding]**

Bind the annotated type to itself with singleton lifetime.

**[Binding(** *Type* serviceType **)]**

Bind the annotated type to `serviceType` with singleton lifetime.

**[Binding(** *BindType* bindType **)]**

Bind the annotated type to itself with `bindType` lifetime.

**[Binding(** *Type* serviceType, *BindType* bindType **)]**

Bind the annotated type to `serviceType` with `bindType` lifetime.

**[AsTransient]**

Bind the annotated type to itself with transient lifetime.

**[AsTransient(** *Type* serviceType **)]**

Bind the annotated type to `serviceType` with transient lifetime.

**[AsScoped]**

Bind the annotated type to itself with scoped lifetime.

**[AsScoped(** *Type* serviceType **)]**

Bind the annotated type to `serviceType` with scoped lifetime.

**[AsSingleton]**

Bind the annotated type to itself with singleton lifetime.

**[AsSingleton(** *Type* serviceType **)]**

Bind the annotated type to `serviceType` with singleton lifetime.

**[Options]**

Bind the annotated type as `T` in `IOptions<T>` against the `IConfigurationRoot`.

**[Options(** *string* subSectionName **)]**

Bind the annotated type as `T` in `IOptions<T>` against the named `IConfiguration` subsection.

### Method Attributes

**[Binding]**

Bind the annotated method as a factory closure to its return type with singleton lifetime.

**[Binding(** *Type* serviceType **)]**

Bind the annotated method as a factory closure to `serviceType` with singleton lifetime.

**[Binding(** *BindType* bindType **)]**

Bind the annotated method as a factory closure to its return type with `bindType` lifetime.

**[Binding(** *Type* serviceType, *BindType* bindType **)]**

Bind the annotated method as a factory closure to `serviceType` with `bindType` lifetime.

**[Factory]**

Bind the annotated method as a factory delegate to the delegate's return value with singleton lifetime.

**[Factory(** *BindType* bindType **)]**

Bind the annotated method as a factory delegate to the delegate's return value with `bindType` lifetime.

**[TransientFactory]**

Bind the annotated method, as a factory delegate, to the delegate's return value with transient lifetime.

**[ScopedFactory]**

Bind the annotated method, as a factory delegate, to the delegate's return value with scoped lifetime.

**[SingletonFactory]**

Bind the annotated method, as a factory delegate, to the delegate's return value with singleton lifetime.

# Bindings and Factories

Dependency Injection is a big topic and central to ASP.NET Core development.

If you're unsure about why you should care, the definitive resource is [Dependency Injection by Mark Seeman](https://www.amazon.com/Dependency-Injection-NET-Mark-Seemann/dp/1935182501) but feel free to just google it.

The DI container described in this document is the one used in ASP.NET Core, but the concepts are the same for just about any modern container.

Skip to the end to learn how to use our DI attributes, `[Binding]`, `[Factory]` and friends.

## Dependency Injection

DI is all about fetching instances of:

- Some concrete type
- Some concrete type which implements some interface

You depend on a DI container to create those instances because your code needs them. 

The code _depends_ on instances of those types.

### Container Configuration

Configuring a DI container consists of telling it how to make instances of types and interfaces.

If you'll ever want an instance of `Foo`, you have to, ahead of time, tell the container how to provide it. If you want an instance of some type which implements `IUseful`, then you have to previously have told the container how to provide it.

The way you tell the container how to do this is by "binding" the desired types or interfaces to either:

- Concrete types (`di.AsSingleton<Foo>()` or `di.AsSingleton<IUseful, Foo>()`)
- Factories which produce instances of concrete types (`di.AsSingleton<Foo>(sp => new Foo())`)

### Dependencies

Once the container knows how to produce instances, your code can request them. If your code _depended_ on using an instance of `Foo`, it could ask the container for one:

    var foo = sp.GetService<Foo>();

Similarly, if your code depended on having an instance of `IUseful` but didn't care which implementation is used, it can again ask the container:

    var useful = sp.GetService<IUseful>()

In this case, the container would create an instance of `Foo` since we told it to bind `IUseful` to `Foo`.

### Injection

But what if `Foo`, or any other dependency, has constructor parameters? That is, what if `Foo` has its _own_ dependencies?

The container can call `Foo`'s constructor _if all of the parameter types are also bound in the container_. The container simply makes instances of those first, and then passes them to `Foo`s constructor. If the parameters have constructor parameters of their own, the container can in turn satisfy those dependencies too -- as long as all the required types and interfaces have been bound. 

Properly configured, a DI container can produce your program's entire object graph

### Factory Closures

In a way, a concrete type's constructor can be thought of a factory - in that new instances can be made by calling it. 

However, what if the container can't provide all of the constructor parameters for a given type? We can't just use a simple binding. Instead, we must provide a factory closure that helps the container do the work of providing those unbound dependencies.

Imagine `Foo` takes an `ILogger` and an `int` which configures the type somehow. We can assume the `ILogger` interface is bound usefully. However, instead of binding `int` in the container, we can instead bind `Foo` to a factory closure, which is just a simple lambda:

    di.AsSingleton<Foo>(sp => { 
        var logger = sp.GetService<ILogger>();
        return new Foo(logger, randomNumber());
    });

When the container must produce an instance of `Foo` it will call this closure. The closure uses the container to resolve the `ILogger` dependency. But we're telling it how to provide the `int` dependency. 

In this case, the container will only ever call the closure once, to produce a single instance, and always return that one. This is thanks to binding `Foo` to the factory via the `AsSingleton` method.

### Lifetimes

Binding a type or interface in the container is done via some method which also specifies the lifetime of instances created with that binding:

- `di.AsSingleton` : Only one instance is created, then always returned
- `di.AsTransient` : A new instance is produced every time
- `di.AsScoped`    : A new instance is provided on each web-request

In the above example, by changing the lifetime to transient, a new random number is produced each time an instance of `Foo` is provided:


    di.AsTransient<Foo>(sp => { 
        var logger = sp.GetService<ILogger>();
        return new Foo(logger, randomNumber());
    });

### Factory Delegates

Factory closures allow us to help the container generate unbound dependencies. By binding a factory closure with a transient lifetime, those dependencies can even vary each time they're generated.

But what if we have some data we want to pass in during the creation of a dependency? The factory closure can only take an `IServiceProvider` and only the container can call it.

#### Delegates

What if the type we bound in the container was not a concrete type or interface, but a function type, or in .NET-lingo, a delegate? What if we bound a delegate in the container which took our runtime data and produced an instance of our dependency?

Imagine that `Foo` depends on `ILogger` and `Stream`. The container can satisfy the `ILogger` dependency, but we only have `Stream` instances at runtime. For example, inside an ASP view we might have the `Stream` representing a form-submitted file.

#### Binding a Factory Delegate

If we have a `Stream` and need a `Foo`, instead of binding `Foo` directly, we can instead bind a delegate `Func<Stream, Foo>`. We'll bind it as as singleton since we don't need multiple copies of the same function:

    di.AsSingleton<Func<Stream, Foo>>(sp => {
        var logger = sp.GetService<ILogger>();
        return stream => new Foo(logger, stream);
    });

The factory delegate is just the function returned from the factory closure. The logger is captured from the parent factory closure, and the stream is provided via the parameter:

    stream => new Foo(logger, stream);    

By binding the delegate type to a factory closure which returns our `Foo`-making factory delegate we can now use the container to get an instance of our factory delegate:

    var fooFactory = sp.GetService<Func<Stream, Foo>>();
    var foo = fooFactory(someStream);

Of course, if the type that owns this code is itself having its dependencies injected by the container, it can just specify the factory delegate as a dependency:

    public class Bar {
        Func<Stream, Foo> fooFactory;

        public Bar(Func<Stream, Foo> fooFactory) {
            this.fooFactory = fooFactory;
        }
    }

Now it doesn't need to interact with the container directly by calling `GetService` on it.

## [Binding] Attribute

The `[Binding]` attribute allows you to bind a concrete type in the container. The simplest use is by applying it to a class to bind the concrete class directly:

    [Binding]
    public class Foo { }

It can also be used to bind an interface to the type:

    [Binding(typeof(IUseful))]
    public class Foo : IUseful { }

By default the lifetime is transient but can be configured:

    [Binding(BindType.Singleton, typeof(IUseful))]
    public class Foo : IUseful { }


Methods can also be utilized to implement simple factory closures:

    public class Foo : IUseful {
        // fields and constructor omitted

        [Binding(typeof(IUseful))]
        public static Foo FooFactory(IServiceProvider sp) {
            var logger = sp.GetService<ILogger>()
            return new Foo(logger, "Hello World);
        }
    }

### Shorthand Attributes

In addition to `[Binding]`, the `[AsSingleton]`, `[AsScoped]` and `[AsTransient]` attributes all do the obvious thing.


## Using the [Factory] attribute

The `[Factory]` attribute allows you to bind a factory delegate in the container. The simplest use is by applying it to a static function which takes an `IServiceProvider` and returns the delegate:

    public class Foo {
        ILogger logger;
        Stream stream;

        public Foo(ILogger logger, Stream stream) {
            this.logger = logger;
            this.stream = stream;
        }

        [Factory]
        public static Func<Stream, Foo> FooFactory(IServiceProvider sp) {
            var logger = sp.GetService<ILogger>();
            return stream => new Foo(logger, stream);
        }
    }


Now dependants can depend on the delegate:

    public class Bar {
        Func<Stream, Foo> fooFactory;

        public Bar(Func<Stream, foo> fooFactory) {
            this.fooFactory = fooFactory;
        }

        public FooizeFileStream(Stream stream) {
            var foo = fooFactory(stream);
            // ...
        }
    }

`Bar` receives a `Func<Stream, Foo>` factory delegate from the container, which it can use to pass `Stream` instances to get `Foo` instances. Each `Foo` instance will have its `ILogger` properly injected thanks to our factory implementation.

### Shorthand Attributes

In addition to `[Factory]`, the `[SingletonFactory]`, `[ScopedFactory]` and `[TransientFactory]` attributes all do the obvious thing.


# Options

Wouldn't it be great if you could automatically have specific configuration data automatically injected into classes? By combining `Microsoft.Extensions.Configuration` and `Microsoft.Extensions.Options` exactly that can be done.

An in-depth walkthrough of the Options Pattern won't be provided here, but it can be read about in the aspnet core documentation:

https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-2.2

## [Options] Attribute

The `[Options]` attribute allows you to bind a concrete class to the `IOptions<T>` interface.

```cs
[Options]
public class FooOptions { 
    public string Bar { get; }
}
```

When `IOptions<FooOptions>` is injected, it will be initialized by your application's `IConfigurationRoot`:

```cs
[Binding]
public class FooService {

    public FooService(IOptions<FooOptions> options) {
        Console.WriteLine($"FooService option Bar = {_options.Value.Bar}");
    }
}
```

The alternative form `[Options("Foo")]` will be initialized by the `Foo` subsection of your application's `IConfigurationRoot`:

```cs
[Options("Foo")]
public class FooOptions { 
    public string Bar { get; }
}
```

If you were using configuration from the environment, you'd need an environment variable named "Foo__Bar" to specify the value. If you were using JSON you'd need a `{"Foo": {"Bar": 42}}` like structure, and so on.

Any configuration sub-section depth can be achieved by separating levels of the configuration with a `:` colon:


```cs
[Options("Foo:Auth")]
public class FooOptions { 
    public string Token { get; }
}
```

Now you'd need an environment variable `Foo__Auth__Token` for example.