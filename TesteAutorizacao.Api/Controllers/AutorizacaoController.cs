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
        private readonly IUsuarioRepository _usuarioRepository;
        public AutorizacaoController(IConfiguration configuration, IUsuarioRepository usuarioRepository) : base(configuration)
        {
            _usuarioRepository = usuarioRepository;
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("Inserir")]
        public IActionResult Inserir(Usuario valor)
        {
            return ExecutaActionResult(() => _usuarioRepository.Inserir(ref valor), true);
        }
    }
}
