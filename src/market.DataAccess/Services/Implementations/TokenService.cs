using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using market.Core.Interfaces.Services;
using market.Core.Models.Identity;

namespace market.DataAccess.Services.Implementations;

public sealed class TokenService(IConfiguration configuration) : ITokenService
{
    private readonly IConfiguration _configuration = configuration;

    public string CreateAccessToken(User user, IReadOnlyList<string> roles)
    {
        var jwt = _configuration.GetSection("Authentication:Jwt");
        var issuer = jwt["Issuer"] ?? "market";
        var audience = jwt["Audience"] ?? "market";
        var signingKey = jwt["SigningKey"];
        var minutes = int.TryParse(jwt["AccessTokenMinutes"], out var m) ? m : 15;

        var keyBytes = string.IsNullOrWhiteSpace(signingKey)
            ? RandomNumberGenerator.GetBytes(32)
            : Encoding.UTF8.GetBytes(signingKey);

        var credentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.FullName ?? user.Email)
        };
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public (string RawToken, string TokenHash, DateTimeOffset ExpiresAtUtc) CreateRefreshToken(int lifetimeDays)
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var raw = Convert.ToBase64String(bytes);
        var hash = HashToken(raw);
        var expires = DateTimeOffset.UtcNow.AddDays(lifetimeDays);
        return (raw, hash, expires);
    }

    public string HashToken(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public bool VerifyTokenHash(string rawToken, string storedHash)
        => string.Equals(HashToken(rawToken), storedHash, StringComparison.Ordinal);
}