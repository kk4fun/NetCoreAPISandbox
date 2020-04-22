namespace NetCoreApiSandbox.Infrastructure.Security
{
    #region

    using System;
    using System.Globalization;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;

    #endregion

    public class JwtTokenGenerator: IJwtTokenGenerator
    {
        private readonly JwtIssuerOptions _jwtOptions;

        public JwtTokenGenerator(IOptions<JwtIssuerOptions> jwtOptions)
        {
            this._jwtOptions = jwtOptions.Value;
        }

        #region IJwtTokenGenerator Members

        public async Task<string> CreateToken(string username)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, await this._jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat,
                          new DateTimeOffset(this._jwtOptions.IssuedAt).ToUnixTimeSeconds().ToString(CultureInfo.CurrentCulture),
                          ClaimValueTypes.Integer64)
            };

            var jwt = new JwtSecurityToken(this._jwtOptions.Issuer,
                                           this._jwtOptions.Audience,
                                           claims,
                                           this._jwtOptions.NotBefore,
                                           this._jwtOptions.Expiration,
                                           this._jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        #endregion
    }
}
