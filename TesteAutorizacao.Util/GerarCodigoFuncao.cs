using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteAutorizacao.Util
{
    public class GerarCodigoFuncao
    {
        public static string GerarCodigoSeisDigitos() 
        {
            string code = DateTime.Now.ToString("ssddmm");
            return string.Concat(code[5], code[0], code[3], code[1], code[2], code[4]);
        }
    }
}
