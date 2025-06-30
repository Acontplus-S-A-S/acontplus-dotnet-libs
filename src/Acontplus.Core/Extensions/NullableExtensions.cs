namespace Acontplus.Core.Extensions;

public static class NullableExtensions
{
    /// <summary>
    /// Returns the value of a nullable bool if it has a value; otherwise, returns the original.
    /// </summary>
    public static bool OrKeep(this bool? source, bool original) =>
        source ?? original;

    /// <summary>
    /// Returns the value of a nullable int if it has a value; otherwise, returns the original.
    /// </summary>
    public static int OrKeep(this int? source, int original) =>
        source ?? original;

    /// <summary>
    /// Returns the value of a nullable long if it has a value; otherwise, returns the original.
    /// </summary>
    public static long OrKeep(this long? source, long original) =>
        source ?? original;

    /// <summary>
    /// Returns the value of a nullable decimal if it has a value; otherwise, returns the original.
    /// </summary>
    public static decimal OrKeep(this decimal? source, decimal original) =>
        source ?? original;

    /// <summary>
    /// Returns the value of a nullable double if it has a value; otherwise, returns the original.
    /// </summary>
    public static double OrKeep(this double? source, double original) =>
        source ?? original;

    /// <summary>
    /// Returns the value of a nullable DateTime if it has a value; otherwise, returns the original.
    /// </summary>
    public static DateTime OrKeep(this DateTime? source, DateTime original) =>
        source ?? original;

    /// <summary>
    /// Returns the new string if it is not null or whitespace; otherwise, returns the original.
    /// </summary>
    public static string OrKeep(this string source, string original) =>
        !string.IsNullOrWhiteSpace(source) ? source : original;

    /// <summary>
    /// Returns the new IEnumerable if it is not null or empty; otherwise, returns the original.
    /// </summary>
    public static IEnumerable<T> OrKeep<T>(this IEnumerable<T> source, IEnumerable<T> original) =>
        source != null && source.Any() ? source : original;
}
