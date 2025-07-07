using Acontplus.Core.Abstractions.Persistence;
using Acontplus.Core.DTOs.Requests;
using Acontplus.TestApplication.Interfaces;
using System.Data;

namespace Acontplus.TestApplication.Services;

public class CustomerService(IAdoRepository adoRepository) : ICustomerService
{
    public async Task<DataTable> GetByIdCardAsync(Dictionary<string, object> parameters)
    {
        var options = new CommandOptionsDto
        {
            CommandTimeout = 0,
            WithTableNames = false
        };
        var ds = await adoRepository.GetDataSetAsync("Customer.Customer_IDCard_Get", parameters, options);
        return ds.Tables[0];
    }
}