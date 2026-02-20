using FluentAssertions;
using InversionOfControl.Model.Factory;
using InversionOfControl.Service.Bootstrap;
using InversionOfControl.Tests.Mock;
using InversionOfControl.Tests.Mock.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace InversionOfControl.Tests;

public class Tests
{
    [Test]
    public void TestRegistrationOfService()
    {
        var bootstrapRegister = new BootstrapRegister();
        var serviceCollection = new ServiceCollection();
        bootstrapRegister.RegisterBootstrapServices(serviceCollection, Substitute.For<IConfiguration>(), Substitute.For<ILoggingBuilder>(), typeof(BoostrapServiceMock).Assembly);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var baseService = serviceProvider.GetRequiredService<IBaseService>();
        baseService.Name.Should().Be("Test");
    }

    [Test]
    public void TestRegistrationOfServiceConditional()
    {
        var bootstrapRegister = new BootstrapRegister();
        var serviceCollection = new ServiceCollection();
        bootstrapRegister.RegisterBootstrapServices(serviceCollection, Substitute.For<IConfiguration>(), Substitute.For<ILoggingBuilder>(), typeof(BoostrapServiceMock).Assembly);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var baseService = serviceProvider.GetService<BootstrapServiceMockConditional.IShouldntExists>();

        baseService.Should().BeNull();
    }


    [Test]
    public void TestFactoriesRegistration()
    {
        var bootstrapRegister = new BootstrapRegister();
        var serviceCollection = new ServiceCollection();
        bootstrapRegister.RegisterBootstrapServices(serviceCollection, Substitute.For<IConfiguration>(), Substitute.For<ILoggingBuilder>(), typeof(BoostrapServiceMock).Assembly);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var factory1 = serviceProvider.GetRequiredService<EnumFactory<FactType, IFactoryServiceMock>>();
        factory1.Should().NotBeNull();
        factory1.GetService(FactType.Foo).Should().BeOfType<FooService>();
        factory1.GetService(FactType.Test).Should().BeOfType<TestService>();

        var factory2 = serviceProvider.GetRequiredService<EnumFactory<FactType2, IFactoryServiceMock2>>();
        factory2.Should().NotBeNull();
        factory2.GetService(FactType2.Foo).Should().BeOfType<FooService2>();
        factory2.GetService(FactType2.Test).Should().BeOfType<TestService2>();
    }

    [Test]
    public void TestFactoriesDefaultToSingleton()
    {
        var bootstrapRegister = new BootstrapRegister();
        var serviceCollection = new ServiceCollection();
        bootstrapRegister.RegisterBootstrapServices(serviceCollection, Substitute.For<IConfiguration>(), Substitute.For<ILoggingBuilder>(), typeof(BoostrapServiceMock).Assembly);

        // Factories without [EnumServiceLifetime] should default to Singleton
        var factoryDescriptor = serviceCollection.First(d => d.ServiceType == typeof(EnumFactory<FactType, IFactoryServiceMock>));
        factoryDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);

        // Service implementations should also be Singleton by default
        var serviceDescriptor = serviceCollection.First(d => d.ImplementationType == typeof(FooService));
        serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Test]
    public void TestFactoryLifetimeAttributeOverridesToScoped()
    {
        var bootstrapRegister = new BootstrapRegister();
        var serviceCollection = new ServiceCollection();
        bootstrapRegister.RegisterBootstrapServices(serviceCollection, Substitute.For<IConfiguration>(), Substitute.For<ILoggingBuilder>(), typeof(BoostrapServiceMock).Assembly);

        // Factory with [EnumServiceLifetime(Scoped)] should be Scoped
        var factoryDescriptor = serviceCollection.First(d => d.ServiceType == typeof(EnumFactory<FactType3, IFactoryServiceMock3>));
        factoryDescriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);

        // Service implementations under a Scoped factory should also be Scoped
        var alphaDescriptor = serviceCollection.First(d => d.ImplementationType == typeof(AlphaService));
        alphaDescriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);

        var betaDescriptor = serviceCollection.First(d => d.ImplementationType == typeof(BetaService));
        betaDescriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Test]
    public void TestScopedFactoryConcreteSubclassIsResolvable()
    {
        var bootstrapRegister = new BootstrapRegister();
        var serviceCollection = new ServiceCollection();
        bootstrapRegister.RegisterBootstrapServices(serviceCollection, Substitute.For<IConfiguration>(), Substitute.For<ILoggingBuilder>(), typeof(BoostrapServiceMock).Assembly);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Concrete subclass should be resolvable
        using var scope = serviceProvider.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<ScopedFactoryMock>();
        factory.Should().NotBeNull();
        factory.GetService(FactType3.Alpha).Should().BeOfType<AlphaService>();
        factory.GetService(FactType3.Beta).Should().BeOfType<BetaService>();
    }

    [Test]
    public void TestScopedFactoryResolvesDifferentInstancesPerScope()
    {
        var bootstrapRegister = new BootstrapRegister();
        var serviceCollection = new ServiceCollection();
        bootstrapRegister.RegisterBootstrapServices(serviceCollection, Substitute.For<IConfiguration>(), Substitute.For<ILoggingBuilder>(), typeof(BoostrapServiceMock).Assembly);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        ScopedFactoryMock factory1;
        ScopedFactoryMock factory2;

        using (var scope1 = serviceProvider.CreateScope())
        {
            factory1 = scope1.ServiceProvider.GetRequiredService<ScopedFactoryMock>();
        }

        using (var scope2 = serviceProvider.CreateScope())
        {
            factory2 = scope2.ServiceProvider.GetRequiredService<ScopedFactoryMock>();
        }

        factory1.Should().NotBeSameAs(factory2);
    }
}