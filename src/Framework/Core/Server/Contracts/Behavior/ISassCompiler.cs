namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface ISassCompiler
{
    String? Compile(String scss, IEnumerable<String> includePaths);
}