using Acontplus.Core.Domain.Common.Results;

namespace Acontplus.TestApplication.Interfaces;

/// <summary>
/// Service interface for testing exception handling from business layer.
/// Methods return Result<T, DomainError|DomainErrors> instead of throwing.
/// </summary>
public interface IBusinessExceptionTestService
{
    Task<Result<object, DomainErrors>> ValidateEmailAsync(string email);

    Task<Result<CustomerModel, DomainError>> GetCustomerAsync(int id);

    Task<Result<CustomerModel, DomainError>> CreateCustomerAsync(string email);

    Task<Result<object, DomainError>> ExecuteDatabaseOperationAsync();

    Task<Result<object, DomainError>> ProcessComplexOperationAsync();

    Task<Result<CustomerModel, DomainError>> GetCustomerWithDeepStackAsync(int id);

    Task<Result<object, DomainError>> AsyncOperationThatFailsAsync();

    Task<Result<object, DomainError>> TaskRunOperationAsync();

    Task<Result<CustomerModel, DomainError>> GetValidCustomerAsync(int id);
}
