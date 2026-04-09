using System.Diagnostics;
using DartSassHost;
using DartSassHost.Helpers;
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Core;

namespace Crudspa.Framework.Core.Server.Services;

public class SassCompilerDartSass(ILogger<SassCompilerDartSass> logger) : ISassCompiler
{
    static SassCompilerDartSass()
    {
        var switcher = JsEngineSwitcher.Current;
        switcher.EngineFactories.AddChakraCore();
        switcher.DefaultEngineName = ChakraCoreJsEngine.EngineName;
    }

    public String? Compile(String scss, IEnumerable<String> includePaths)
    {
        try
        {
            var options = new CompilationOptions
            {
                OutputStyle = OutputStyle.Compressed,
                SourceMap = false,
                IncludePaths = includePaths.ToArray(),
            };

            using var compiler = new SassCompiler(options);
            var result = compiler.Compile(scss, false);

            if (result.CompiledContent.HasNothing())
                throw new SassCompilationException("Sass compiler failed silently.");

            return result.CompiledContent;
        }
        catch (SassCompilationException compilationException)
        {
            if (Debugger.IsAttached) Debugger.Break();
            logger.LogError(compilationException, "Final Sass compilation failure: {details}", SassErrorHelpers.GenerateErrorDetails(compilationException));
            return null;
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached) Debugger.Break();
            logger.LogError(ex, $"Unhandled exception in {nameof(SassCompilerDartSass)}.");
            return null;
        }
    }
}