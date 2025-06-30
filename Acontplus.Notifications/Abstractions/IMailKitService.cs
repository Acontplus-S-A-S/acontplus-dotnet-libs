using Acontplus.Notifications.Models;

namespace Acontplus.Notifications.Abstractions;

public interface IMailKitService
{
    Task<bool> SendAsync(EmailModel email, CancellationToken ct = default);
}
