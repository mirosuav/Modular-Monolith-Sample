using Serilog.Core;
using Serilog.Events;

namespace RiverBooks.SharedKernel.Extensions;

/// <summary>
/// A Serilog enricher that adds module name to the context properties
/// </summary>
public class ModuleNameEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var moduleName = RetreiveModuleName(logEvent);
        if (moduleName is not null)
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(
                    "ModuleName", moduleName));
    }

    private static string? RetreiveModuleName(LogEvent logEvent)
    {
        if (logEvent.Properties.TryGetValue("SourceContext", out var sourceContextProp))
        {
            var sourceContext = sourceContextProp.ToString().Trim('"');
            if (sourceContext.StartsWith(nameof(RiverBooks)))
                return sourceContext.Split('.')[1];
            else
                return sourceContext;
        }

        return null;
    }
}
