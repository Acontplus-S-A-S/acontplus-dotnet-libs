namespace Acontplus.Core.Extensions;


public static class PagedResultExtensions
{
    public static Dictionary<string, string?> BuildPaginationLinks<T>(
        this PagedResult<T> result,
        string baseRoute,
        int pageSize)
    {
        string Link(int page) => $"{baseRoute}?page={page}&size={pageSize}";

        return new()
        {
            ["first"] = Link(1),
            ["previous"] = result.HasPreviousPage ? Link(result.PageIndex - 1) : null,
            ["next"] = result.HasNextPage ? Link(result.PageIndex + 1) : null,
            ["last"] = Link(result.TotalPages)
        };
    }
}