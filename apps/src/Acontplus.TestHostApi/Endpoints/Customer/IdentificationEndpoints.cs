using Acontplus.TestApplication.DTOs;
using Acontplus.TestApplication.Interfaces;
using Acontplus.Utilities.Extensions;

namespace Acontplus.TestHostApi.Endpoints.Customer;

public static class IdentificationEndpoints
{
    public static void MapIdentificationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customer")
            .WithTags("Customer Identification");

        group.MapGet("/by-id-card", GetByIdCard)
            .WithName("GetIdCardInformation")
            .Produces<CustomerDto>()
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetByIdCard(
        string idCard,
        bool sriOnly,
        ICustomerService customerService)
    {
        var customer = await customerService.GetByIdCardAsync(idCard, sriOnly);

        return customer.ToMinimalApiResult();
    }
}