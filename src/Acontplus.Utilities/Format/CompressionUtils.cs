using System.IO.Compression;

namespace Acontplus.Utilities.Format;

public static class CompressionUtils
{
    // Deflate Compression
    public static byte[] CompressDeflate(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        using var memoryStream = new MemoryStream();
        using (var deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, true))
        {
            deflateStream.Write(data, 0, data.Length);
        }

        return memoryStream.ToArray();
    }

    // Deflate Decompression
    public static byte[] DecompressDeflate(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        using var compressStream = new MemoryStream(data);
        using var deflateStream = new DeflateStream(compressStream, CompressionMode.Decompress);
        using var decompressedStream = new MemoryStream();
        {
            deflateStream.CopyTo(decompressedStream);
            return decompressedStream.ToArray();
        }
    }

    // GZip Compression
    public static byte[] CompressGZip(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        using var memoryStream = new MemoryStream();
        using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
        {
            gzipStream.Write(data, 0, data.Length);
        }

        return memoryStream.ToArray();
    }

    // GZip Decompression
    public static byte[] DecompressGZip(byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        using var compressStream = new MemoryStream(data);
        using var gzipStream = new GZipStream(compressStream, CompressionMode.Decompress);
        using var decompressedStream = new MemoryStream();
        {
            gzipStream.CopyTo(decompressedStream);
            return decompressedStream.ToArray();
        }
    }

    public static void DecompressColumn(DataSet dataSet, string tableName, string compressedColumnName,
        string decompressedColumnName)
    {
        ArgumentNullException.ThrowIfNull(dataSet);

        if (!dataSet.Tables.Contains(tableName))
        {
            throw new ArgumentException($"Table '{tableName}' does not exist in the DataSet.");
        }

        var table = dataSet.Tables[tableName];
        DecompressColumn(table, compressedColumnName, decompressedColumnName);
    }

    public static void DecompressColumn(DataTable? table, string compressedColumnName, string decompressedColumnName)
    {
        if (table == null)
        {
            throw new ArgumentNullException(nameof(table));
        }

        if (!table.Columns.Contains(compressedColumnName))
        {
            throw new ArgumentException($"Column '{compressedColumnName}' does not exist in the DataTable.");
        }

        // Add a new column to store the decompressed content if it doesn't exist
        if (!table.Columns.Contains(decompressedColumnName))
        {
            table.Columns.Add(decompressedColumnName, typeof(string));
        }

        // Loop through each row in the DataTable
        foreach (DataRow row in table.Rows)
        {
            if (row[compressedColumnName] is byte[] compressedData)
            {
                var decompressedString = Encoding.UTF8.GetString(DecompressGZip(compressedData));
                row[decompressedColumnName] = decompressedString;
            }
        }
    }
}
