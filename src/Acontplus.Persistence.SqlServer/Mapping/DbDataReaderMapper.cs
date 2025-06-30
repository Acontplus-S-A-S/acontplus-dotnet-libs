using System.Reflection;

namespace Acontplus.Persistence.SqlServer.Mapping;

public static class DbDataReaderMapper
{
    /// <summary>
    /// Maps a SqlDataReader to a List of entities of type T
    /// </summary>
    /// <typeparam name="T">The type to map to. Must be a reference type or a struct.</typeparam>
    /// <param name="reader">The SqlDataReader containing the data</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A list of mapped entities</returns>
    public static async Task<List<T>> ToListAsync<T>(SqlDataReader reader, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var result = new List<T>();
        var type = typeof(T);

        // Get type's properties
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // Get the column mapping
        var columnMap = BuildColumnMapping(reader, properties);

        // Read all rows
        while (await reader.ReadAsync(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Create new instance of T
            var instance = (T)Activator.CreateInstance(type);

            foreach (var (columnName, property) in columnMap)
            {
                var ordinal = reader.GetOrdinal(columnName);

                // Skip if column value is null
                if (await reader.IsDBNullAsync(ordinal, cancellationToken))
                {
                    continue;
                }

                try
                {
                    var value = reader.GetValue(ordinal);
                    var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    if (value != DBNull.Value)
                    {
                        var convertedValue = Convert.ChangeType(value, propertyType);
                        property.SetValue(instance, convertedValue);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Error mapping column '{columnName}' to property '{property.Name}' of type '{type.FullName}'",
                        ex);
                }
            }

            result.Add(instance);
        }

        return result;
    }

    private static Dictionary<string, PropertyInfo> BuildColumnMapping(
        SqlDataReader reader,
        PropertyInfo[] properties)
    {
        var columnMap = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < reader.FieldCount; i++)
        {
            var columnName = reader.GetName(i);
            if (string.IsNullOrEmpty(columnName))
            {
                continue;
            }

            var property = properties.FirstOrDefault(p =>
                string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase));

            if (property != null)
            {
                columnMap[columnName] = property;
            }
        }

        return columnMap;
    }

    // Keep the original synchronous method for backward compatibility
    public static List<T> ToList<T>(IDataReader rdr)
    {
        var ret = new List<T>();
        var typ = typeof(T);
        var properties = typ.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var columns = new Dictionary<string, PropertyInfo>();

        // Map columns to properties (case-insensitive)
        for (var index = 0; index < rdr.FieldCount; index++)
        {
            var columnName = rdr.GetName(index);
            var prop = properties.FirstOrDefault(p =>
                string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase));
            if (prop != null)
            {
                columns.Add(columnName, prop);
            }
        }

        // Loop through all records
        while (rdr.Read())
        {
            // Create new instance of T
            var entity = Activator.CreateInstance<T>();

            // Assign values to the entity's properties
            foreach (var column in columns)
            {
                var property = column.Value;
                var columnValue = rdr[column.Key];

                if (columnValue != DBNull.Value)
                {
                    // Handle nullable types (if applicable)
                    var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    // Convert the column value to the property type if needed
                    var safeValue = Convert.ChangeType(columnValue, propertyType);

                    property.SetValue(entity, safeValue);
                }
                else
                {
                    property.SetValue(entity, null); // Handle DBNull
                }
            }

            ret.Add(entity);
        }

        return ret;
    }
}
