using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Services
{
    public class JwtService
    {
        private string _secretKey;
        private string _issuer;
        private string _audience;

        public JwtService()
        {
            _secretKey = "YourSuperSecretKeyAtLeast32CharactersLong!";
            _issuer = "WebApi";
            _audience = "WebApi";
        }

        public void Configure(string secretKey, string issuer = null, string audience = null)
        {
            _secretKey = secretKey;
            if (issuer != null) _issuer = issuer;
            if (audience != null) _audience = audience;
        }

        public Dictionary<string, object> GenerateToken(Dictionary<string, object> claims, int expireMinutes = 60)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtClaims = new List<Claim>();
            foreach (var kv in claims)
            {
                if (kv.Value != null)
                    jwtClaims.Add(new Claim(kv.Key, kv.Value.ToString()));
            }

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: jwtClaims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: credentials);

            var handler = new JwtSecurityTokenHandler();
            var tokenString = handler.WriteToken(token);

            return new Dictionary<string, object>
            {
                ["token"] = tokenString,
                ["expiresAt"] = token.ValidTo,
                ["issuedAt"] = token.IssuedAt,
                ["issuer"] = _issuer,
                ["audience"] = _audience
            };
        }

        public Dictionary<string, object> ValidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = handler.ValidateToken(token, validationParams, out var validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;

                return new Dictionary<string, object>
                {
                    ["valid"] = true,
                    ["claims"] = principal.Claims.ToDictionary(c => c.Type, c => (object)c.Value),
                    ["expiresAt"] = jwtToken?.ValidTo,
                    ["issuedAt"] = jwtToken?.IssuedAt
                };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    ["valid"] = false,
                    ["error"] = ex.Message
                };
            }
        }

        public Dictionary<string, string> DecodeToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = handler.ReadJwtToken(token);
                return jwtToken.Claims.ToDictionary(c => c.Type, c => c.Value);
            }
            catch
            {
                return new Dictionary<string, string>();
            }
        }
    }
}
