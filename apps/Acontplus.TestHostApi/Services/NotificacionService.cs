namespace Acontplus.TestHostApi.Services;

public interface IEmailService
{
    public Task<DataTable> GetAsync(int quantity);
    public Task<int> UpdateAsync(int id, string estado, string msgError = null);
}

public sealed class NotificacionService(IAdoRepository repository) : IEmailService
{
    public async Task<DataTable> GetAsync(int cantidad)
    {
        var options = new CommandOptionsDto
        {
            CommandTimeout = 0,
            WithTableNames = false
        };
        var ds = await repository.GetDataSetAsync("Common.EmailQueue_Serv_Get",
            new Dictionary<string, object> { { "quantity", cantidad } }, options);
        return ds.Tables[0];
    }

    public async Task<int> UpdateAsync(int id, string estado, string msgError)
    {
        var parameters = new Dictionary<string, object>
        {
            { "id", id },
            { "estado", estado },
            { "msgError", msgError ?? "" }
        };
        return await repository.ExecuteNonQueryAsync("App.Notificacion_Serv_Update", parameters);
    }
}
