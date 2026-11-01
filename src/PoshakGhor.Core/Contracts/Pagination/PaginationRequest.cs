namespace PoshakGhor.Core.Contracts.Pagination;

public sealed record PaginationRequest(int Page = 1, int PageSize = 20)
{
    public int Offset => (int)Math.Min(((long)Math.Max(Page, 1) - 1) * Limit, int.MaxValue);
    public int Limit => Math.Clamp(PageSize, 1, 100);
}
