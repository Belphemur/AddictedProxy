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
}