using InversionOfControl.Model.Factory;

namespace InversionOfControl.Tests.Mock.Factory;

public enum FactType2
{
    Test,
    Foo
}
public interface IFactoryServiceMock2 : IEnumService<FactType2>
{
    
}

public class TestService2 : IFactoryServiceMock2
{
    public FactType2 Enum => FactType2.Test;
}

public class FooService2 : IFactoryServiceMock2
{
    public FactType2 Enum => FactType2.Foo;
}

