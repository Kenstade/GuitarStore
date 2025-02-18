using GuitarStore.Identity.Models;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using GuitarStore.Data;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace GuitarStore.Identity.Jwt;

internal sealed class JwtService(IOptions<JwtOptions> jwtOptions, AppDbContext dbContext)
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly AppDbContext _dbContext = dbContext;
    public string GenerateJwtToken(User user, IReadOnlyList<string>? roles = null)
    {
        var jwtClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email)
        };

        if(roles != null)
        jwtClaims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var handler = new JsonWebTokenHandler();

        var token = handler.CreateToken(new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(jwtClaims),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.TokenLifeTimeMinutes)
        });

        return token;
    }
    public async Task<string> GenerateRefreshToken(Guid userId, string? token = null)
    {
        var refreshToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.UserId == userId && rt.Token == token);

        if (refreshToken == null)
        {
            token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = token,
                ExpiredAt = DateTime.UtcNow.AddDays(1)
            };

            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            if(refreshToken.ExpiredAt < DateTime.UtcNow)
                throw new ApplicationException($"refresh token {refreshToken.Token} is invalid");

            token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            refreshToken.Token = token;
            refreshToken.ExpiredAt = DateTime.UtcNow.AddDays(1);

            _dbContext.RefreshTokens.Update(refreshToken);
            await _dbContext.SaveChangesAsync();
        }

        await RemoveOldRefreshTokens(userId);

        return refreshToken.Token;
    }
    
    private Task<int> RemoveOldRefreshTokens(Guid userId)
    {
        var refreshTokens = _dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId)
            .ToList()
            .RemoveAll(x => x.ExpiredAt < DateTime.UtcNow);

        return _dbContext.SaveChangesAsync();
    }

    public ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = _jwtOptions.Audience,
            ValidIssuer = _jwtOptions.Issuer,
            IssuerSigningKey =  new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(
            token,
            tokenValidationParameters,
            out SecurityToken securityToken);

        if (securityToken == null) throw new SecurityTokenException("Invalid access token");

        return principal;
    }

}
