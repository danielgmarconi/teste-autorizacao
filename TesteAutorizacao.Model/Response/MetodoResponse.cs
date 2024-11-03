using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteAutorizacao.Model.Response
{
    public class MetodoResponse
    {
        public bool sucesso { get; set; }
        public int codigoStatus { get; set; }
        public string mensagem { get; set; }
        public object resposta { get; set; }

    }
}
