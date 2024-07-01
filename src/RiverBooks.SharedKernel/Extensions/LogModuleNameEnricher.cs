using Serilog.Core;
using Serilog.Events;
using System.Reflection;

namespace RiverBooks.SharedKernel.Extensions;

/// <summary>
/// A Serilog enricher that adds module name to the context properties
/// </summary>
public class LogModuleNameEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "Module", GetCallingAssemblyModuleName()));
    }

    private static string GetCallingAssemblyModuleName()
    {
        var assemblyName = Assembly.GetCallingAssembly().GetName()?.Name ?? nameof(RiverBooks);

        return assemblyName.Split('.')[1];
    }
}
