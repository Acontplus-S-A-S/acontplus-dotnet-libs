namespace Acontplus.Utilities.Json;

/// <summary>
/// JSON manipulation extensions
/// </summary>
public static class JsonManipulationExtensions
{
    /// <summary>
    /// Extension method to validate JSON string
    /// </summary>
    public static bool IsValidJson(this string json) => JsonHelper.ValidateJson(json).IsValid;

    /// <summary>
    /// Extension method to get JSON property
    /// </summary>
    public static T? GetJsonProperty<T>(this string json, string propertyName)
        => JsonHelper.GetJsonProperty<T>(json, propertyName);

    /// <summary>
    /// Extension method to merge JSON
    /// </summary>
    public static string MergeJson(this string json1, string json2)
        => JsonHelper.MergeJson(json1, json2);

    /// <summary>
    /// Extension method to compare JSON
    /// </summary>
    public static bool JsonEquals(this string json1, string json2)
        => JsonHelper.AreEqual(json1, json2);
}