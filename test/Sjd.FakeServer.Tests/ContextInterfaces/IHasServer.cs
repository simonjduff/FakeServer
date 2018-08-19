namespace Sjd.FakeServer.Tests.ContextInterfaces
{
    public interface IHasServer
    {
        FakeServer FakeServer { get; set; }
    }
}