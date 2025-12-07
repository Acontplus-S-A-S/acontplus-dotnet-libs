namespace Demo.Application.Dtos;

/// <summary>
/// Request model for customer operations.
/// </summary>
public class CustomerRequest
{
    public string? Email { get; set; }
    public string? Name { get; set; }
}

/// <summary>
/// Customer model for responses.
/// </summary>
public class CustomerModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Status { get; set; } = "Active";
}
