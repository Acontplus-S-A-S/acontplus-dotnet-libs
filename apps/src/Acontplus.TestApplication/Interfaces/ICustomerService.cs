using System.Data;
using Acontplus.Core.Domain.Common;
using Acontplus.TestApplication.DTOs;

namespace Acontplus.TestApplication.Interfaces;

public interface ICustomerService
{
    Task<Result<CustomerDto, DomainErrors>> GetByIdCardAsync(string idCard, bool sriOnly = false);
}