namespace Acontplus.Core.Extensions;

public static class NullableExtensions
{
    public static bool IsNull<T>(this T? value) where T : class => value is null;

    public static bool IsNotNull<T>(this T? value) where T : class => value is not null;

    public static T OrDefault<T>(this T? value, T defaultValue) where T : class => value ?? defaultValue;

    public static T OrThrow<T>(this T? value, Exception exception) where T : class => value ?? throw exception;

    public static T OrThrow<T>(this T? value, Func<Exception> exceptionFactory) where T : class => value ?? throw exceptionFactory();

    public static T OrThrow<T>(this T? value, string message) where T : class => value ?? throw new ArgumentNullException(nameof(value), message);

    public static T OrThrow<T>(this T? value, Func<string> messageFactory) where T : class => value ?? throw new ArgumentNullException(nameof(value), messageFactory());
}
