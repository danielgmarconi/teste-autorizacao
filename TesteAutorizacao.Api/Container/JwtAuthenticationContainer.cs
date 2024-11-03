using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TesteAutorizacao.Api.Container
{
    public class JwtAuthenticationContainer
    {
        public static void RegisterServices(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(cfg => { cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                                        cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                                        cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x => {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("TokenPrivada").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }
    }
}
