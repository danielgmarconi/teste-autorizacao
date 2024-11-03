 using Microsoft.AspNetCore.Mvc;
using TesteAutorizacao.Model.Response;

namespace TesteAutorizacao.Api.Controllers
{
    public class AutorizacaoControllerBase : ControllerBase
    {
        private IConfiguration configuration;
        protected string GetHeaderOpenId => Request.Headers.TryGetValue("OpenId", out var value) ? value.First() : null;
        protected string GetOpenId => configuration.GetSection("OpenId").Value;
        public AutorizacaoControllerBase(IConfiguration _configuration = null) => configuration = _configuration;
        protected IActionResult ExecutaActionResult<T>(Func<T> function, bool IsOpenId = false) where T : class
        {
            try
            {
                var resultado = new MetodoResponse();
                if (IsOpenId && !GetOpenId.Equals(GetHeaderOpenId))
                {
                    resultado.codigoStatus = 403;
                }
                else
                    resultado = function() as MetodoResponse;
                return StatusCode(resultado.codigoStatus, resultado);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
