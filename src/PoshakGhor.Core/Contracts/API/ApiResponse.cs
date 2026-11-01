namespace PoshakGhor.Core.Contracts.API;

public sealed record ApiResponse<T>(bool Success, T? Data, string? Message = null);
