namespace Acontplus.TestApplication.Services;

public sealed class NotificacionService(IAdoRepository repository) : IEmailService
{
    public async Task<DataTable> GetAsync(int quantity)
    {
        var options = new CommandOptionsDto
        {
            CommandTimeout = 0,
            WithTableNames = false
        };
        var ds = await repository.GetDataSetAsync("Common.EmailQueue_Serv_Get",
            new Dictionary<string, object> { { "quantity", quantity } }, options);
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

