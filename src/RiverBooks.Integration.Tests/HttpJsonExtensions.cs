using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RiverBooks.Integration.Tests;

internal static class HttpJsonExtensions
{
    public static JsonSerializerOptions JsonOpts = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static T? DeserializeAs<T>(this string json) => JsonSerializer.Deserialize<T>(json, JsonOpts);

    public static string ToJson(this object obj) => JsonSerializer.Serialize(obj);

    public static StringContent ToStringContentUtf(this object obj) =>
        obj.ToJson().ToStringContentUtf();

    public static StringContent ToStringContentUtf(this string obj) =>
        new StringContent(obj, Encoding.UTF8, "application/json");
}

