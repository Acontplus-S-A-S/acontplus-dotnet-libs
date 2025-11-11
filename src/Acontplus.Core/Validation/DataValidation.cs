namespace Acontplus.Core.Validation;

public static class DataValidation
{
    public static object ToDbNullOrDefault(this object obj)
    {
        return obj ?? DBNull.Value;
    }

    public static bool DataTableIsNull(DataTable dt)
    {
        var isNull = dt is not { Rows.Count: > 0 };
        return isNull;
    }

    public static bool DataSetIsNull(DataSet ds, bool removeEmptyDt = false)
    {
        switch (removeEmptyDt)
        {
            case true:
                {
                    var tablesToRemove = ds.Tables.Cast<DataTable>().Where(dt => dt.Rows.Count == 0).ToList();

                    foreach (var dt in tablesToRemove)
                    {
                        ds.Tables.Remove(dt);
                    }

                    break;
                }
        }

        return ds == null || ds.Tables.Count == 0;
    }

    public static bool IsXml(string input)
    {
        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(input);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static string RemoveSpecialCharacters(string text)
    {
        return Regex.Replace(text, "[^0-9A-Za-z _-]", "");
    }

    public static bool IsValidJson(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return false;
        }

        try
        {
            using (JsonDocument.Parse(jsonString))
            {
                return true;
            }
        }
        catch (JsonException)
        {
            return false;
        }
    }

    public static string ValidateIpAddress(string ipAddress)
    {
        if (string.IsNullOrEmpty(ipAddress))
        {
            return "0.0.0.0";
        }

        switch (ipAddress.Length)
        {
            case > 13:
                {
                    var extractedIpAddress = ipAddress.Substring(13);
                    return IPAddress.TryParse(extractedIpAddress, out _) ? extractedIpAddress : "0.0.0.0";
                }
            default:
                return ipAddress ?? "0.0.0.0";
        }
    }
}
