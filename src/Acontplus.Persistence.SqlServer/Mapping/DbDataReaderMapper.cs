using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Acontplus.Persistence.SqlServer.Mapping;

public static class DbDataReaderMapper
{
    /// <summary>
    /// Maps a DbDataReader to a List of entities of type T with support for records and init-only properties
    /// </summary>
    public static async Task<List<T>> ToListAsync<T>(this DbDataReader reader, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var result = new List<T>();
        var type = typeof(T);
        var isRecord = IsRecordType(type);

        // Get all public instance properties including init-only
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                           .Where(p => IsWritable(p, isRecord))
                           .ToArray();

        var columnMap = BuildColumnMapping(reader, properties);

        await foreach (var item in MapRecordsAsync<T>(reader, columnMap, cancellationToken))
        {
            result.Add(item);
        }

        return result;
    }

    private static async IAsyncEnumerable<T> MapRecordsAsync<T>(
        DbDataReader reader,
        Dictionary<string, PropertyInfo> columnMap,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var columnNames = columnMap.Keys.ToArray();

        while (await reader.ReadAsync(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var instance = CreateInstance<T>();

            foreach (var columnName in columnNames)
            {
                var ordinal = reader.GetOrdinal(columnName);
                if (await reader.IsDBNullAsync(ordinal, cancellationToken))
                    continue;

                var value = reader.GetValue(ordinal);
                var property = columnMap[columnName];
                var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                try
                {
                    var convertedValue = ConvertValue(value, propertyType);
                    property.SetValue(instance, convertedValue);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Error mapping column '{columnName}' to property '{property.Name}'", ex);
                }
            }

            yield return instance;
        }
    }

    private static Dictionary<string, PropertyInfo> BuildColumnMapping(
        DbDataReader reader,
        PropertyInfo[] properties)
    {
        var columnMap = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < reader.FieldCount; i++)
        {
            var columnName = reader.GetName(i);
            if (string.IsNullOrEmpty(columnName)) continue;

            var property = properties.FirstOrDefault(p =>
                string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase));

            if (property != null)
            {
                columnMap[columnName] = property;
            }
        }

        return columnMap;
    }

    private static T CreateInstance<T>()
    {
        var type = typeof(T);

        if (type.IsValueType)
            return default!;

        if (type.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance<T>();

        // Fallback for records and types without parameterless constructors
        return (T)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
    }

    private static object ConvertValue(object value, Type targetType)
    {
        try
        {
            if (targetType.IsEnum)
                return Enum.ToObject(targetType, value);

            if (targetType == typeof(Guid))
                return value is string s ? Guid.Parse(s) : (Guid)value;

            if (targetType == typeof(DateTimeOffset))
                return value is DateTime dt ? new DateTimeOffset(dt) : (DateTimeOffset)value;

            return Convert.ChangeType(value, targetType);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to convert value '{value}' to type {targetType.Name}", ex);
        }
    }

    private static bool IsRecordType(Type type)
    {
        // Check for compiler-generated attributes or Clone method
        return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute)) ||
               type.GetMethods().Any(m => m.Name == "<Clone>$");
    }

    private static bool IsWritable(PropertyInfo prop, bool isRecord)
    {
        // For records, we consider init-only properties as writable during construction
        if (isRecord)
        {
            var setMethod = prop.GetSetMethod(nonPublic: true);
            return setMethod != null && (setMethod.IsPublic || setMethod.IsAssembly);
        }

        return prop.CanWrite;
    }

    // Synchronous version
    public static List<T> ToList<T>(this DbDataReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var result = new List<T>();
        var type = typeof(T);
        var isRecord = IsRecordType(type);

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                           .Where(p => IsWritable(p, isRecord))
                           .ToArray();

        var columnMap = BuildColumnMapping(reader, properties);

        while (reader.Read())
        {
            var instance = CreateInstance<T>();

            foreach (var columnName in columnMap.Keys)
            {
                var ordinal = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(ordinal))
                    continue;

                var value = reader.GetValue(ordinal);
                var property = columnMap[columnName];
                var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                try
                {
                    var convertedValue = ConvertValue(value, propertyType);
                    property.SetValue(instance, convertedValue);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Error mapping column '{columnName}' to property '{property.Name}'", ex);
                }
            }

            result.Add(instance);
        }

        return result;
    }
}