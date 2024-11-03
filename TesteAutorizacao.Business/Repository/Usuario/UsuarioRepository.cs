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
        public List<Usuario> UserList(ref Usuario valor)
        {
            return DataAccessLayer.ExecuteReader<Usuario>(connectionStringSql, System.Data.CommandType.StoredProcedure, "spUsuarioSelect", valor); 
        }
        public bool IsExists(Usuario value)
        {
            return UserList(ref value).Count() > 0;
        }

        public MetodoResponse CodeResend(ref string email)
        {
            var result = new MetodoResponse();
            //var user = new Users() { Email = email };
            //user = UserList(ref user).FirstOrDefault();
            //var code = CodeGeneratorFunction.GenerateSixDigits();
            //var emailTemplateRequest = new EmailTemplateRequest();
            //emailTemplateRequest.Email = user.Email;
            //emailTemplateRequest.EmailTypeSend = "newletter";
            //emailTemplateRequest.TemplateName = "sendcode";
            //emailTemplateRequest.SubjectList = new string[] { user.Email.Split("@")[0] };
            //emailTemplateRequest.MessageList = new string[] { user.Email.Split("@")[0], code };
            //user.CreationData = null;
            //user.ModificationDate = null;
            //user.ModificationUserId = user.Id;
            //user.RecoveryCode = code;
            //user.RecoveryCodeExpiration = DateTime.Now.AddMinutes(recoveryCodeExpiration);
            //DataAccessLayer.ExecuteScalar(connectionStringSql, System.Data.CommandType.StoredProcedure, "spUsersUpdate", user);
            //new VerisEmailService(GetServiceConfig("VerisEmail"), _httpClientFactory).SendEmailTemplate(emailTemplateRequest);
            //result.response = user;
            //result.success = true;
            //result.statusCode = 200;
            return result;
        }
    }
}
