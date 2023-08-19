using InversionOfControl.Model.Factory;

namespace InversionOfControl.Tests.Mock.Factory;

public enum FactType
{
    Test,
    Foo
}
public interface IFactoryServiceMock : IEnumService<FactType>
{
    
}

public class TestService : IFactoryServiceMock
{
    public FactType Enum => FactType.Test;
}

public class FooService : IFactoryServiceMock
{
    public FactType Enum => FactType.Foo;
}

