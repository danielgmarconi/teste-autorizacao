using TesteAutorizacao.Business.Repository;

namespace TesteAutorizacao.Api.Container
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection service)
        {
            service.AddHttpClient();
            service.AddScoped<IUsuarioRepository, UsuarioRepository>();
        }
    }
}
