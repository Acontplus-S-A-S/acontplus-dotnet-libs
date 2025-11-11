using Acontplus.Core.Domain.Common.Results;
using Acontplus.TestApplication.Dtos;

namespace Acontplus.TestApplication.Interfaces;

public interface ICustomerService
{
    Task<Result<CustomerDto, DomainErrors>> GetByIdCardAsync(string idCard, bool sriOnly = false);
}
