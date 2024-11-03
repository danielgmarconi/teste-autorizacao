using Microsoft.Extensions.Configuration;
using TesteAutorizacao.Model;

namespace TesteAutorizacao.Business.Repository
{
    public class RepositoryBase
    {
        protected readonly IConfiguration configuration;
        protected readonly string connectionStringSql;
        protected readonly string TokenPrivada;
        protected readonly int MinutosToken;
        protected readonly int MinutosTokenRenovacao;
        protected readonly string ChaveSenhaPrivada;
        protected readonly List<ServiceWebApi> ServicesLista;
        public RepositoryBase(IConfiguration _configuration)
        {
            configuration = _configuration;
            ServicesLista = _configuration.GetSection("Services").Get<List<ServiceWebApi>>();
            connectionStringSql = _configuration.GetConnectionString("SqlServerDB");
            TokenPrivada = configuration.GetSection("TokenPrivada").Value;
            MinutosToken = int.Parse(_configuration.GetSection("MinutosToken").Value);
            MinutosTokenRenovacao = int.Parse(_configuration.GetSection("MinutosTokenRenovacao").Value);
            ChaveSenhaPrivada = configuration.GetSection("ChaveSenhaPrivada").Value;
        }

        protected ServiceWebApi GetServiceConfig(string name)
        {
            return ServicesLista.FirstOrDefault(x => x.Name.ToLower().Equals(name.ToLower()));
        }
    }
}
