using System.Reflection;

namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IEmbeddedResourceService
{
    Task<String> ReadText(Assembly assembly, String resourceName);
    Task<Byte[]> ReadBytes(Assembly assembly, String resourceName);
    Byte[] ReadBytesNow(Assembly assembly, String resourceName);
}