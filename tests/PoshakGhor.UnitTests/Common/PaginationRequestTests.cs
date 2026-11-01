using PoshakGhor.Core.Contracts.Pagination;

namespace PoshakGhor.UnitTests.Common;

public sealed class PaginationRequestTests
{
    [Fact]
    public void Limit_IsClampedToMaximum()
    {
        var request = new PaginationRequest(1, 500);
        Assert.Equal(100, request.Limit);
    }

    [Fact]
    public void Offset_UsesNormalizedPageAndLimit()
    {
        var request = new PaginationRequest(3, 10);
        Assert.Equal(20, request.Offset);
    }
}
