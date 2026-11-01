namespace PoshakGhor.Core.Contracts.Pagination;

public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int Page, int PageSize, long TotalCount);
