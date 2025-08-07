using System.Data;

namespace Acontplus.Core.DTOs.Responses;

public record SpResponse<T>
{
    public T? Data { get; init; }
    public DataTable? DataTable { get; init; }
    public DataSet? DataSet { get; init; }
    public int RowsAffected { get; init; }
    public bool HasData { get; init; }
    public string? Message { get; init; }
    public bool IsSuccess { get; init; }

    public static SpResponse<T> Success(T data, int rowsAffected = 0, string? message = null)
    {
        return new SpResponse<T>
        {
            Data = data,
            RowsAffected = rowsAffected,
            HasData = data is not null,
            Message = message ?? "Operation completed successfully.",
            IsSuccess = true
        };
    }

    public static SpResponse<T> Success(DataTable dataTable, int rowsAffected = 0, string? message = null)
    {
        return new SpResponse<T>
        {
            DataTable = dataTable,
            RowsAffected = rowsAffected,
            HasData = dataTable?.Rows.Count > 0,
            Message = message ?? "Operation completed successfully.",
            IsSuccess = true
        };
    }

    public static SpResponse<T> Success(DataSet dataSet, int rowsAffected = 0, string? message = null)
    {
        return new SpResponse<T>
        {
            DataSet = dataSet,
            RowsAffected = rowsAffected,
            HasData = dataSet?.Tables.Count > 0,
            Message = message ?? "Operation completed successfully.",
            IsSuccess = true
        };
    }

    public static SpResponse<T> Failure(string message, int rowsAffected = 0)
    {
        return new SpResponse<T>
        {
            RowsAffected = rowsAffected,
            HasData = false,
            Message = message,
            IsSuccess = false
        };
    }

    public static SpResponse<T> Failure(Exception exception, int rowsAffected = 0)
    {
        return new SpResponse<T>
        {
            RowsAffected = rowsAffected,
            HasData = false,
            Message = exception.Message,
            IsSuccess = false
        };
    }
}
