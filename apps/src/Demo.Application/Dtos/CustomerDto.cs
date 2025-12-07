namespace Demo.Application.Dtos;

public record CustomerDto(
    string IdCard,
    string BusinessName,
    string? TradeName,
    string? Address,
    string? Email,
    string? Telephone);
