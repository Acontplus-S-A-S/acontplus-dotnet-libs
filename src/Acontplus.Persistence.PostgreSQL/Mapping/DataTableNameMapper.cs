namespace Acontplus.Persistence.PostgreSQL.Mapping;

public static class DataTableNameMapper
{
    public static async Task ProcessTableNames(NpgsqlCommand cmd, DataSet ds)
    {
        var tableNames = cmd.Parameters["@tableNames"].Value?.ToString()?.Split(',');
        if (tableNames == null) return;

        // Parallel.ForEach is okay here as it's a CPU-bound operation on in-memory data
        await Task.Run(() =>
        {
            Parallel.ForEach(tableNames, (tableName, _, index) =>
            {
                if (string.IsNullOrEmpty(tableName)) return;
                // Ensure index is within bounds to prevent ArgumentOutOfRangeException
                if (index >= 0 && index < ds.Tables.Count)
                {
                    ds.Tables[(int)index].TableName = tableName;
                }
            });
        });
    }
}