using Service.Contracts;
using Tournament.Services;

namespace Tournament.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureServiceLayerServices(this IServiceCollection services)
        {
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<ITournamentService, TournamentService>();
            services.AddScoped<IGameService, GameService>();

            services.AddScoped(provider => new Lazy<ITournamentService>(() => provider.GetRequiredService<ITournamentService>()));
            services.AddScoped(provider => new Lazy<IGameService>(() => provider.GetRequiredService<IGameService>()));

        }
    }
}
