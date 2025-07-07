using System.Data;

namespace Acontplus.TestApplication.Interfaces;

public interface ICustomerService
{
    Task<DataTable> GetByIdCardAsync(Dictionary<string, object> parameters);
}