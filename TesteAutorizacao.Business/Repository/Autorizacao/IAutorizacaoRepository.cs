

using TesteAutorizacao.Model;
using TesteAutorizacao.Model.Response;

namespace TesteAutorizacao.Business.Repository
{
    public interface IAutorizacaoRepository
    {
        MetodoResponse AutenticacaoUsuario(ref AutorizacaoUsuario valor);
        public MetodoResponse RenovacaoAutenticacaoUsuario(ref string value);
    }
}
