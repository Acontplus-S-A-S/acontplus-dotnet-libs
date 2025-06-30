namespace Acontplus.Core.DTOs.Responses;

public class BaseResponseDto
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? CreatedByUserId { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public int? UpdatedByUserId { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
}
