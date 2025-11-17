using System.ComponentModel.DataAnnotations;

namespace Acontplus.Core.Dtos.Requests;

public class CommandOptionsDto
{
    // Default command timeout in seconds. 0 indicates no timeout.
    // Null means use ADO.NET default (usually 30 seconds).
    [Range(0, 3600)]
    public int? CommandTimeout { get; set; }

    // Specifies how the command text is to be interpreted (StoredProcedure or Text)
    public CommandType CommandType { get; set; } = CommandType.StoredProcedure; // Sensible default for your usage

    // Specific to GetDataSetAsync
    public bool WithTableNames { get; set; } = true;

    [Range(1, 1000)]
    public int TableNamesLength { get; set; } = 500;
    /// <summary>
    /// Controls filter parameter strategy:
    /// - false (default): Individual parameters for raw SQL (better performance)
    /// - true: JSON serialized parameters for stored procedures (flexibility)
    /// - null: Auto-detect based on CommandType (JSON for StoredProcedure)
    /// </summary>
    public bool? UseJsonFilters { get; set; }
}

