using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using TesteAutorizacao.Model;
using TesteAutorizacao.Model.Response;
using TesteAutorizacao.Util;
using VerisUserAuthentication.Business.Validators;

namespace TesteAutorizacao.Business.Repository
{


    public class UsuarioRepository : RepositoryBase, IUsuarioRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public UsuarioRepository(IConfiguration configuration,
                                 IHttpClientFactory httpClientFactory) : base(configuration) 
        {
            _httpClientFactory = httpClientFactory;
        }
        public MetodoResponse Inserir(ref Usuario valor)
        {
            var resultado = new MetodoResponse();
            var validation = new UsuarioRegistroValidator(this).Validate(valor);
            if (!validation.IsValid)
            {
                resultado.codigoStatus = 500;
                resultado.resposta = validation.Errors.Select(x => x.ErrorMessage).ToArray<string>();
                return resultado;
            }
            valor.ChaveSenha = CriptografiaFuncao.GerarChavePublica();
            valor.Senha = CriptografiaFuncao.Encriptar(valor.Senha, valor.ChaveSenha, ChaveSenhaPrivada);
            valor.Id = Int64.Parse(DataAccessLayer.ExecuteScalar(connectionStringSql, System.Data.CommandType.StoredProcedure, "spUsuarioInsert", valor).ToString());
            resultado.resposta = valor;
            resultado.sucesso = true;
            resultado.codigoStatus = 201;
            return resultado;
        }
        public MetodoResponse Buscar(ref Usuario valor)
        {
            var resultado = new MetodoResponse();
            resultado.resposta = UserList(ref valor);
            resultado.sucesso = true;
            resultado.codigoStatus = 200;
            return resultado;
        }
        public List<Usuario> UserList(ref Usuario valor)
        {
            return DataAccessLayer.ExecuteReader<Usuario>(connectionStringSql, System.Data.CommandType.StoredProcedure, "spUsuarioSelect", valor); 
        }
        public bool IsExists(Usuario valor)
        {
            return UserList(ref valor).Count() > 0;
        }
    }
}
