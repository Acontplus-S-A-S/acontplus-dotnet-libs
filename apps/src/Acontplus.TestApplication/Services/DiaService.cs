using Acontplus.Core.Domain.Common.Results;
using Acontplus.TestApplication.Dtos;
using Acontplus.Utilities.Mapping;
using Microsoft.Extensions.Logging;

namespace Acontplus.TestApplication.Services
{
    public class DiaService : IDiaService
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Dia> _diaRepository;

        public DiaService(IUnitOfWork unitOfWork, ILogger<DiaService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _diaRepository = _unitOfWork.GetRepository<Dia>();
        }

        public async Task<Result<Dia, DomainError>> CreateAsync(CreateDiaDto createDiaDto)
        {
            var dia = ObjectMapper.Map<CreateDiaDto, Dia>(createDiaDto);
            await _diaRepository.AddAsync(dia);
            await _unitOfWork.SaveChangesAsync();

            return Result<Dia, DomainError>.Success(dia);
        }

        public async Task<Result<Dia, DomainError>> UpdateAsync(int id, UpdateDiaDto updateDiaDto)
        {

            var dia = await _diaRepository.GetByIdAsync(updateDiaDto.Id);
            dia.Name = updateDiaDto.Name;
            dia.Description = updateDiaDto.Description;
            await _diaRepository.UpdateAsync(dia);
            await _unitOfWork.SaveChangesAsync();

            return Result<Dia, DomainError>.Success(dia);
        }
    }
}

