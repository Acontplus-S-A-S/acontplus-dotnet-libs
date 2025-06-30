namespace Acontplus.Notifications.Entities;

public class WhatsAppUsage : BaseEntity
{
    public int CompanyId { get; set; }
    public int Used { get; set; }
    public int Limit { get; set; }
    public bool Unlimited { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
