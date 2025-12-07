namespace Demo.Application.Interfaces
{
    public interface IDiaService
    {
        Task<Result<Dia, DomainError>> CreateAsync(CreateDiaDto createDiaDto);
        Task<Result<Dia, DomainError>> UpdateAsync(int id, UpdateDiaDto updateDiaDto);
    }
}

