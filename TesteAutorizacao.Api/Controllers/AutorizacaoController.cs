using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TesteAutorizacao.Business.Repository;
using TesteAutorizacao.Model;

namespace TesteAutorizacao.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutorizacaoController : AutorizacaoControllerBase
    {
        private readonly IAutorizacaoRepository _autorizacaoRepository;
        public AutorizacaoController(IConfiguration configuration, IAutorizacaoRepository autorizacaoRepository) : base(configuration)
        {
            _autorizacaoRepository = autorizacaoRepository;
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("AutenticacaoUsuario")]
        public IActionResult AutenticacaoUsuario(AutorizacaoUsuario valor)
        {
            return ExecutaActionResult(() => _autorizacaoRepository.AutenticacaoUsuario(ref valor), true);
        }
        [HttpPost]
        [Authorize]
        [Route("RenovacaoAutenticacaoUsuario")]
        public IActionResult RenovacaoAutenticacaoUsuario()
        {
            var token = ObterTokenCabecalho;
            return ExecutaActionResult(() => _autorizacaoRepository.RenovacaoAutenticacaoUsuario(ref token), false);
        }
    }
}
