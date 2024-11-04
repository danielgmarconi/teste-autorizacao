using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TesteAutorizacao.Business.Repository;
using TesteAutorizacao.Model;

namespace TesteAutorizacao.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : AutorizacaoControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        public UsuarioController(IConfiguration configuration, IUsuarioRepository usuarioRepository) : base(configuration)
        {
            _usuarioRepository = usuarioRepository;
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("Criar")]
        public IActionResult Criar(Usuario valor)
        {
            return ExecutaActionResult(() => _usuarioRepository.Inserir(ref valor), true);
        }
        [HttpPost]
        [Authorize]
        [Route("Buscar")]
        public IActionResult Buscar(Usuario valor)
        {
            return ExecutaActionResult(() => _usuarioRepository.Buscar(ref valor), false);
        }
    }
}
