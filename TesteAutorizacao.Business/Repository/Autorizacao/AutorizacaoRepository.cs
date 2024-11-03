using Microsoft.Extensions.Configuration;
using TesteAutorizacao.Business.Repository;
using TesteAutorizacao.Model;
using TesteAutorizacao.Model.Response;
using TesteAutorizacao.Util;
namespace TesteAutorizacao.Business.Repository
{
    public class AutorizacaoRepository : RepositoryBase, IAutorizacaoRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public AutorizacaoRepository(IConfiguration configuration,
                                     IHttpClientFactory httpClientFactory) : base(configuration) 
        {
            _httpClientFactory = httpClientFactory;
        }
        public MetodoResponse AutenticacaoUsuario(ref AutorizacaoUsuario valor)
        {
            var resultado = new MetodoResponse();
            List<Usuario> list = DataAccessLayer.ExecuteReader<Usuario>(connectionStringSql, System.Data.CommandType.StoredProcedure, "spUsuarioSelect", new Usuario() { Email = valor.Email });
            if (list.Count > 0 && CriptografiaFuncao.Decriptar(list[0].Senha, list[0].ChaveSenha, ChaveSenhaPrivada).Equals(valor.Senha))
            {
                var resposta = new AutorizacaoResponse();
                resposta.usuarioId = list[0].Id.Value;
                resposta.email = list[0].Email;
                resposta.tokenUsuario = JwtTokenFuncao.GerarTokenUsuario(list[0].Id.Value, list[0].Email, TokenPrivada, MinutosToken);
                resposta.tokenRenovacao = JwtTokenFuncao.GerarTokenRenovacao(list[0].Id.Value, list[0].Email, TokenPrivada, MinutosTokenRenovacao);
                resultado.resposta = resposta;
                resultado.codigoStatus = 200;
            }
            else
                resultado.codigoStatus = 401;             
            return resultado;
        }
        public MetodoResponse RenovacaoAutenticacaoUsuario(ref string value)
        {
            var resultado = new MetodoResponse();
            var resposta = new AutorizacaoResponse();
            resposta.usuarioId = Int64.Parse(JwtTokenFuncao.ObterClaimValor(value, "userid"));
            resposta.email = JwtTokenFuncao.ObterClaimValor(value, "email");
            resposta.tokenUsuario = JwtTokenFuncao.GerarTokenUsuario(resposta.usuarioId, resposta.email, TokenPrivada, MinutosToken);
            resposta.tokenRenovacao = JwtTokenFuncao.GerarTokenRenovacao(resposta.usuarioId, resposta.email, TokenPrivada, MinutosTokenRenovacao);
            resultado.sucesso = true;
            resultado.codigoStatus = 200;
            resultado.resposta = resposta;
            return resultado;
        }
    }
}
