using Acontplus.Core.Domain.Common.Results;

namespace Acontplus.TestApplication.Interfaces
{
    public interface IDiaService
    {
        Task<Result<Dia, DomainError>> CreateAsync(CreateDiaDto createDiaDto);
        Task<Result<Dia, DomainError>> UpdateAsync(int id, UpdateDiaDto updateDiaDto);
    }
}
