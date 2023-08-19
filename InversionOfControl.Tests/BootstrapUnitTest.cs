using FluentAssertions;
using InversionOfControl.Model;
using InversionOfControl.Model.Factory;
using InversionOfControl.Service.Bootstrap;
using InversionOfControl.Tests.Mock;
using InversionOfControl.Tests.Mock.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace InversionOfControl.Tests;

public class Tests
{
    [Test]
    public void TestRegistrationOfService()
    {
        var bootstrapRegister = new BootstrapRegister();
        var serviceCollection = new ServiceCollection();
        bootstrapRegister.RegisterBootstrapServices(serviceCollection, Substitute.For<IConfiguration>(), typeof(BoostrapServiceMock).Assembly);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var baseService = serviceProvider.GetRequiredService<IBaseService>();
        baseService.Name.Should().Be("Test");
    }

    [Test]
    public void TestRegistrationOfServiceConditional()
    {
        var bootstrapRegister = new BootstrapRegister();
        var serviceCollection = new ServiceCollection();
        bootstrapRegister.RegisterBootstrapServices(serviceCollection, Substitute.For<IConfiguration>(), typeof(BoostrapServiceMock).Assembly);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var baseService = serviceProvider.GetService<BootstrapServiceMockConditional.IShouldntExists>();

        baseService.Should().BeNull();
    }

    [Test]
    public void TestFactoryRegistration()
    {
        var bootstrapRegister = new BootstrapRegister();
        var serviceCollection = new ServiceCollection();
        bootstrapRegister.RegisterBootstrapServices(serviceCollection, Substitute.For<IConfiguration>(), typeof(BoostrapServiceMock).Assembly);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var baseService = serviceProvider.GetRequiredService<EnumFactory<FactType, IFactoryServiceMock>>();
        baseService.Should().NotBeNull();
        baseService.GetService(FactType.Foo).Should().BeOfType<FooService>();
        baseService.GetService(FactType.Test).Should().BeOfType<TestService>();

    }
}