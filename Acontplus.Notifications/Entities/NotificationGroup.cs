namespace Acontplus.Notifications.Entities;

public class NotificationGroup : BaseEntity
{
    public string? Name { get; set; } // Example: "Finance Team", "Company-Wide"
    public int? CompanyId { get; set; } // Nullable: Groups can span multiple companies

    // Relationships
    public ICollection<UserGroup>? UserGroups { get; set; } // Links users to this group
}