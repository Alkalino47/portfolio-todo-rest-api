using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationTests;

internal static class JwtTokenFactory
{
    public const string Issuer = "portfolio-todo-rest-api-tests";
    public const string Audience = "portfolio-todo-rest-api-tests";
    public const string SigningKey = "CHANGE_ME_DEV_ONLY_32_CHARS_MINIMUM_SECRET";

    public static string CreateToken()
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, "integration-test-user"),
                new Claim(ClaimTypes.Name, "Integration Test")
            ],
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
