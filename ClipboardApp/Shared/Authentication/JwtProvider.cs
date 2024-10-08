using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ClipboardApp.Shared.Authentication;

public class JwtProvider(IOptions<JwtOptions> jwtOptions) : IJwtProvider
{
    private readonly JwtOptions _options = jwtOptions.Value;

    public string Generate(string sessionId, int lifetimeHours)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_options.JwtSecret!)),
            SecurityAlgorithms.HmacSha256);

        var claims = new ClaimsIdentity(new[]
        {
            new Claim("sessionId", sessionId)
        });

        var token = new JwtSecurityToken(
            this._options.Issuer,
            this._options.Audience,
            claims.Claims,
            null,
            DateTime.UtcNow.AddHours(lifetimeHours),
            signingCredentials);

        var tokenValue = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return tokenValue;
    }
}
