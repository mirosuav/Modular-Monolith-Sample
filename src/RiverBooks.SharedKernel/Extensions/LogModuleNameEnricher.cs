using Serilog.Context;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
