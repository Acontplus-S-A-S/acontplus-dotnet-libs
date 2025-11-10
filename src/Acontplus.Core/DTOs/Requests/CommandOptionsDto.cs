namespace Acontplus.Core.Dtos.Requests;

public class CommandOptionsDto
{
    // Default command timeout in seconds. 0 indicates no timeout.
    // Null means use ADO.NET default (usually 30 seconds).
    public int? CommandTimeout { get; set; }

    // Specifies how the command text is to be interpreted (StoredProcedure or Text)
    public CommandType CommandType { get; set; } = CommandType.StoredProcedure; // Sensible default for your usage

    // Specific to GetDataSetAsync
    public bool WithTableNames { get; set; } = true;
    public int TableNamesLength { get; set; } = 500;
}

