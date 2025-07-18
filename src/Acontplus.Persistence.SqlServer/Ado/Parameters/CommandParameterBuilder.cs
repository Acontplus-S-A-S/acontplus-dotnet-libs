namespace Acontplus.Persistence.SqlServer.Ado.Parameters;

public static class CommandParameterBuilder
{
    public static void AddParameter(DbCommand cmd, string name, object value)
    {
        var param = cmd.CreateParameter();
        param.ParameterName = name.StartsWith("@") ? name : $"@{name}";
        param.Value = value;
        cmd.Parameters.Add(param);
    }

    public static void AddOutputParameter(SqlCommand cmd, string name, SqlDbType type, int size)
    {
        var param = cmd.CreateParameter();
        param.ParameterName = name;
        param.SqlDbType = type;
        param.Size = size;
        param.Direction = ParameterDirection.Output;
        cmd.Parameters.Add(param);
    }

    public static object GetParameter(SqlCommand command, string parameterName)
    {
        return command.Parameters[parameterName].Value;
    }
}
