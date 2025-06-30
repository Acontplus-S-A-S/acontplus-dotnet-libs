namespace Acontplus.Notifications.Entities;

public class UserGroup : BaseEntity
{
    public int GroupId { get; set; }
    public required NotificationGroup Group { get; set; }
}