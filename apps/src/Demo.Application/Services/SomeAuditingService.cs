namespace Demo.Application.Services;

public class SomeAuditingService
{
    private readonly IUnitOfWork _auditUnitOfWork;

    public SomeAuditingService([FromKeyedServices("audit")] IUnitOfWork auditUnitOfWork)
    {
        _auditUnitOfWork = auditUnitOfWork;
    }

    public async Task LogSomethingAsync()
    {
        // ... lógica que usa la unidad de trabajo de auditoría
        await _auditUnitOfWork.SaveChangesAsync();
    }
}
