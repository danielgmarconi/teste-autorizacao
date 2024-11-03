using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteAutorizacao.Model.Response
{
    public class AutorizacaoResponse
    {
        public Int64 usuarioId { get; set; }
        public string email { get; set; }
        public string tokenUsuario { get; set; }
        public string tokenRenovacao { get; set; }
    }
}
