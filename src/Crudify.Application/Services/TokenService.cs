
namespace Crudify.Application.Services;

public class TokenService(IOptionsMonitor<JwtSettings> jwtConfig, ApplicationContext context, TokenValidationParameters tokenValidationParameters) : ITokenService
{
    public async Task<AuthResult> GenerateToken(ApplicationUser user, IList<string> roles)
    {
        JwtSecurityTokenHandler? jwtTokenHandler = new();

        var key = Encoding.UTF8.GetBytes(jwtConfig.CurrentValue.SecretKey);


        var claims = new List<Claim>
        {
            new("Id", user.Id.ToString()),
            new (JwtRegisteredClaimNames.Email, user.Email),
            new (JwtRegisteredClaimNames.Sub, user.Email),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddSeconds(30),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken? token = jwtTokenHandler.CreateToken(tokenDescriptor);
        string jwtToken = jwtTokenHandler.WriteToken(token);

        
        RefreshToken refreshToken = new RefreshToken()
        {
            JwtId = token.Id,
            IsUsed = false,
            IsRevoked = false,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddMonths(1),
            Token = GetRandomString() + Guid.NewGuid()
        };

        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();



        return new AuthResult()
        {
            Token = jwtToken,
            RefreshToken = refreshToken.Token,
            Success = true,
        };
    }

    public async Task<RefreshTokenResponseDTO> VerifyToken(TokenRequestDTO tokenRequest)
    {
        JwtSecurityTokenHandler? jwtTokenHandler = new JwtSecurityTokenHandler();

        try
        {
            RefreshToken? storedToken = await context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == tokenRequest.RefreshToken);
            if (storedToken == null)
            {
                return new RefreshTokenResponseDTO()
                {
                    Success = false,
                    Errors = new List<string>{
                 "token does not found"
                }
                };
            }
            ClaimsPrincipal? tokenVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, tokenValidationParameters, out var validatedToken); //?

            var jti = tokenVerification.Claims.FirstOrDefault(t => t.Type == JwtRegisteredClaimNames.Jti).Value;

            if (storedToken.JwtId != jti)
            {
                return new RefreshTokenResponseDTO()
                {
                    Success = false,
                    Errors = new List<string>{
                 "token doesn't match"
                }
                };
            }

            long utcExpireDate = long.Parse(tokenVerification.Claims.FirstOrDefault(d => d.Type == JwtRegisteredClaimNames.Exp).Value);

            // UTC to DateTime
            DateTime expireDate = UTCtoDateTime(utcExpireDate);

            Console.WriteLine($"expireDate: {expireDate} - now: {DateTime.UtcNow}");

            if (expireDate > DateTime.UtcNow)
            {
                return new RefreshTokenResponseDTO()
                {
                    Success = false,
                    Errors = new List<string>{
                    "Token not expired"
                }
                };
            }

            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                bool result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);//?

                if (!result)
                {
                    return null;
                }
            }

            if (storedToken.IsUsed)
            {
                return new RefreshTokenResponseDTO()
                {
                    Success = false,
                    Errors = new List<string>{
                 "token used."
                }
                };
            }

            if (storedToken.IsRevoked)
            {
                return new RefreshTokenResponseDTO()
                {
                    Success = false,
                    Errors = new List<string>{
                 "token revoked."
                }
                };
            }

            storedToken.IsUsed = true;
            context.RefreshTokens.Update(storedToken);
            await context.SaveChangesAsync();

            // return token
            return new RefreshTokenResponseDTO()
            {
                UserId = storedToken.UserId,
                Success = true,
            };
        }
        catch (Exception e)
        {

            return new RefreshTokenResponseDTO()
            {
                Errors = new List<string>{
                e.Message
            },
                Success = false
            };
        }
    }

    private DateTime UTCtoDateTime(long unixTimeStamp)
    {
        var datetimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        datetimeVal = datetimeVal.AddSeconds(unixTimeStamp).ToLocalTime();

        return datetimeVal;
    }

    private string GetRandomString()
    {
        Random random = new Random();
        string chars = "ABCDEFGHIJKLMNOPRSTUVYZWX0123456789";
        return new string(Enumerable.Repeat(chars, 35).Select(n => n[new Random().Next(n.Length)]).ToArray());

    }
}
