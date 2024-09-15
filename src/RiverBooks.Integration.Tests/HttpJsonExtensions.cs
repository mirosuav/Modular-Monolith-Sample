using System.Text;
using System.Text.Json;

namespace RiverBooks.Integration.Tests;

internal static class HttpJsonExtensions
{
    public static string ToJson(this object obj) => JsonSerializer.Serialize(obj);

    public static StringContent ToStringContentUtf(this object obj) =>
        obj.ToJson().ToStringContentUtf();

    public static StringContent ToStringContentUtf(this string obj) =>
        new StringContent(obj, Encoding.UTF8, "application/json");
}

