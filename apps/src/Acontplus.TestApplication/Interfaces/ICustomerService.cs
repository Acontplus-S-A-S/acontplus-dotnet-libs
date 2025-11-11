using Acontplus.Core.Domain.Common.Results;

namespace Acontplus.TestApplication.Interfaces;

public interface ICustomerService
{
    Task<Result<CustomerDto, DomainErrors>> GetByIdCardAsync(string idCard, bool sriOnly = false);
}
