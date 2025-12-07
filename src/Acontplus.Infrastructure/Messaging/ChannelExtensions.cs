using System.Threading.Channels;

namespace Acontplus.Infrastructure.Messaging;

/// <summary>
/// Extension methods for System.Threading.Channels to support type-safe channel transformations.
/// </summary>
internal static class ChannelExtensions
{
    /// <summary>
    /// Casts a ChannelReader&lt;object&gt; to a ChannelReader&lt;T&gt; by filtering and transforming items.
    /// Creates a new unbounded channel that only contains items of type T.
    /// </summary>
    /// <typeparam name="T">The target type to cast to.</typeparam>
    /// <param name="reader">The source channel reader containing objects.</param>
    /// <returns>A new ChannelReader&lt;T&gt; that yields only items of type T.</returns>
    public static ChannelReader<T> Cast<T>(this ChannelReader<object> reader) where T : class
    {
        var channel = Channel.CreateUnbounded<T>(new UnboundedChannelOptions
        {
            SingleWriter = false,
            SingleReader = false,
            AllowSynchronousContinuations = false
        });

        _ = Task.Run(async () =>
        {
            try
            {
                await foreach (var item in reader.ReadAllAsync())
                {
                    if (item is T typedItem)
                    {
                        await channel.Writer.WriteAsync(typedItem);
                    }
                }
            }
            catch (ChannelClosedException)
            {
                // Channel was closed, normal termination
            }
            catch (Exception)
            {
                // Log errors if needed in production implementations
                // Silently complete the channel to prevent hanging
            }
            finally
            {
                channel.Writer.Complete();
            }
        });

        return channel.Reader;
    }
}
