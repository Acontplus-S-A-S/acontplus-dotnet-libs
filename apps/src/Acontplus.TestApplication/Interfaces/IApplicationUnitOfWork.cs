using Acontplus.Core.Abstractions.Persistence;

namespace Acontplus.TestApplication.Interfaces;

// Interfaz específica para tu contexto de aplicación
public interface IApplicationUnitOfWork : IUnitOfWork
{
    // Aquí podrías agregar repositorios específicos si los expones a través de la UoW
    // IProductRepository Products { get; }
}