using FluentAssertions;
using InversionOfControl.Model;
using InversionOfControl.Service.Bootstrap;
using InversionOfControl.Tests.Mock;
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
}