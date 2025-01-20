using GuitarStore.Identity.Models;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using GuitarStore.Data;
using Microsoft.EntityFrameworkCore;

namespace GuitarStore.Identity.Jwt;

public class JwtService(IOptions<JwtOptions> jwtOptions, AppDbContext dbContext)
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly AppDbContext _dbContext = dbContext;
    public string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var handler = new JsonWebTokenHandler();

        var token = handler.CreateToken(new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email.ToString())
            ]),
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

}
