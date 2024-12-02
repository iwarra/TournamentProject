using Bogus;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.Entities;
using Tournament.Data.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Tournament.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task SeedDataAsync(this IApplicationBuilder builder)
        {
            using (var scope = builder.ApplicationServices.CreateScope()) 
            {
                var serviceProvider = scope.ServiceProvider;
                var db = serviceProvider.GetRequiredService<TournamentApiContext>();

               await db.Database.MigrateAsync();

                if (await db.TournamentDetails.AnyAsync()) return;

                try
                {
                    var turnaments = GenerateTurnaments(4);
                    await db.AddRangeAsync(turnaments);
                    await db.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private static IEnumerable<TournamentDetails> GenerateTurnaments(int nrOfTurnaments)
        {
            var faker = new Faker<TournamentDetails>("sv").Rules((f, t) =>
            {
                t.Title = $"{GameNames[f.Random.Int(0, GameNames.Length - 1)]} Tournament";
                t.StartDate = f.Date.Between(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));

                t.Games = GenerateGames(f.Random.Int(2, 10));
            });

            return faker.Generate(nrOfTurnaments);
        }

        private static readonly string[] GameNames = { "Football", "Chess", "Basketball", "Tennis" };

        private static ICollection<Game> GenerateGames(int nrOfGames)
        {
         
            var faker = new Faker<Game>("sv").Rules((f, g) =>
            {
                g.Time = f.Date.Between(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1));
                g.Title = GameNames[f.Random.Int(0, GameNames.Length - 1)];
            });

            return faker.Generate(nrOfGames);
        }
    }
}
