using Acontplus.Core.Abstractions.Persistence;
using Acontplus.Core.DTOs.Requests;
using Acontplus.TestApplication.Interfaces;
using System.Data;
using Acontplus.Core.Domain.Common;
using Acontplus.Core.Domain.Enums;
using Acontplus.Core.Domain.Exceptions;
using Acontplus.FactElect.Interfaces.Services;
using Acontplus.TestApplication.DTOs;

namespace Acontplus.TestApplication.Services;

public class CustomerService(
    IAdoRepository adoRepository,
    IRucService rucService,
    ICedulaService cedulaService,
    ISqlExceptionTranslator sqlExceptionTranslator)
    : ICustomerService
{
    public async Task<Result<CustomerDto, DomainErrors>> GetByIdCardAsync(string idCard, bool sriOnly = false)
    {
        try
        {
            // Validate ID card length first
            if (idCard.Length is not (10 or 13))
            {
                var error = DomainError.Validation(
                    code: "INVALID_ID_CARD",
                    message: "The provided ID card is invalid. It must be either 10 or 13 characters long.",
                    target: nameof(idCard),
                    details: new Dictionary<string, object> { ["actualLength"] = idCard.Length });

                return Result<CustomerDto, DomainErrors>.Failure(error);
            }

            if (sriOnly)
            {
                return await GetCustomerSriByIdCardAsync(idCard);
            }

            var parameters = new Dictionary<string, object> { { "idCard", idCard } };
            var customer =
                await adoRepository.QuerySingleOrDefaultAsync<CustomerDto>("Demo.GetCustomerByIdCard", parameters);

            if (customer == null)
            {
                var notFoundError = DomainError.NotFound(
                    code: "CUSTOMER_NOT_FOUND",
                    message: "No customer found with the provided ID card",
                    target: nameof(idCard),
                    details: new Dictionary<string, object> { ["idCard"] = idCard });

                return Result<CustomerDto, DomainErrors>.Failure(notFoundError);
            }

            return Result<CustomerDto, DomainErrors>.Success(customer);
        }
        catch (Exception ex)
        {
            var domainEx = sqlExceptionTranslator.Translate(ex);

            // Handle validation errors
            if (domainEx.ErrorType == ErrorType.Validation)
            {
                var validationErrors = new List<DomainError>
                {
                    DomainError.Validation(
                        code: "ID_CARD_VALIDATION_ERROR",
                        message: "The provided ID card is invalid.",
                        target: nameof(idCard),
                        details: new Dictionary<string, object>
                        {
                            ["idCard"] = idCard,
                            ["error"] = domainEx.Message
                        })
                };

                // If you have multiple validation errors, you can add them to the list
                // validationErrors.Add(anotherError);

                return Result<CustomerDto, DomainErrors>.Failure(DomainErrors.Multiple(validationErrors));
            }

            // Handle transient errors
            if (sqlExceptionTranslator.IsTransient(ex))
            {
                var transientError = DomainError.ServiceUnavailable(
                    code: "DB_TRANSIENT_ERROR",
                    message: "Database temporarily unavailable",
                    details: new Dictionary<string, object>
                    {
                        ["procedure"] = "dbo.sp_Test",
                        ["error"] = ex.Message
                    });

                return Result<CustomerDto, DomainErrors>.Failure(transientError);
            }

            // Fallback to internal error
            var internalError = DomainError.Internal(
                code: "SP_EXECUTION_ERROR",
                message: "Failed to execute stored procedure",
                details: new Dictionary<string, object>
                {
                    ["procedure"] = "dbo.sp_Test",
                    ["error"] = ex.Message
                });

            return Result<CustomerDto, DomainErrors>.Failure(internalError);
        }
    }

    private async Task<Result<CustomerDto, DomainErrors>> GetCustomerSriByIdCardAsync(string idCard)
    {
        if (idCard.Length == 13)
        {
            return await GetRucSriAsync(idCard);
        }

        return await GetCedulaSriAsync(idCard);
    }

    private async Task<Result<CustomerDto, DomainErrors>> GetRucSriAsync(string idCard)
    {
        var rucInfo = await rucService.GetRucSriAsync(idCard);
        var customer = new CustomerDto(IdCard:
            rucInfo.Value.Contribuyente.NumeroRuc,
            BusinessName: rucInfo.Value.Contribuyente.RazonSocial,
            TradeName: rucInfo.Value.Contribuyente.NombreComercial,
            Address: rucInfo.Value.Contribuyente.Direccion,
            Email: rucInfo.Value.Contribuyente.Email,
            Telephone: rucInfo.Value.Contribuyente.Telefono);
        return Result<CustomerDto, DomainErrors>.Success(customer);
    }

    private async Task<Result<CustomerDto, DomainErrors>> GetCedulaSriAsync(string idCard)
    {
        var rucInfo = await cedulaService.GetCedulaSriAsync(idCard);
        var customer = new CustomerDto(IdCard:
            rucInfo.Value.Identificacion,
            BusinessName: rucInfo.Value.NombreCompleto,
            TradeName: string.Empty,
            Address: rucInfo.Value.Direccion,
            Email: rucInfo.Value.Email,
            Telephone: rucInfo.Value.Telefono);
        return Result<CustomerDto, DomainErrors>.Success(customer);
    }
}