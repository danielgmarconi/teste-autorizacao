using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteAutorizacao.Model;
using TesteAutorizacao.Model.Response;

namespace TesteAutorizacao.Business.Repository
{
    public interface IUsuarioRepository
    {
        MetodoResponse Inserir(ref Usuario valor);
        List<Usuario> UserList(ref Usuario valor);
        bool IsExists(Usuario valor);
        MetodoResponse CodeResend(ref string email);
    }
}
