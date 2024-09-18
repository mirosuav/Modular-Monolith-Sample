using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RiverBooks.Integration.Tests;

internal static class HttpJsonExtensions
{
    public static JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static T? DeserializeAs<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json, JsonOpts);
    }

    public static string ToJson(this object obj)
    {
        return JsonSerializer.Serialize(obj);
    }

    public static StringContent ToStringContentUtf(this object obj)
    {
        return obj.ToJson().ToStringContentUtf();
    }

    public static StringContent ToStringContentUtf(this string obj)
    {
        return new StringContent(obj, Encoding.UTF8, "application/json");
    }
}