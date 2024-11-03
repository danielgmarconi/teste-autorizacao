using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TesteAutorizacao.Util
{
    public class JwtTokenFuncao
    {
        public static string GerarTokenUsuario(Int64 userid,
                                               string email,
                                               string privateToken,
                                               int minutesToken)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim("userid", userid.ToString()));
            claims.Add(new Claim("email", email));
            return GerarToken(claims, privateToken, minutesToken);
        }
        public static string GerarTokenRenovacao(Int64 userid,
                                                 string email,
                                                 string privateToken,
                                                 int minutesToken)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim("userid", userid.ToString()));
            claims.Add(new Claim("email", email));
            return GerarToken(claims, privateToken, minutesToken);
        }
        private static string GerarToken(List<Claim> claims,
                                            string privateToken,
                                            int minutesToken)
        {
            var jwtToken = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(minutesToken),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                       Encoding.UTF8.GetBytes(privateToken)
                        ),
                    SecurityAlgorithms.HmacSha256Signature)
                );
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public static DateTime GetValidTo(string token) => ((JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(token)).ValidTo;

        public static string ObterClaimValor(string token, string ClaimNome)
        {
            var securityToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(token);
            return securityToken.Claims.FirstOrDefault(c => c.Type == ClaimNome)?.Value;

        }
    }
}
