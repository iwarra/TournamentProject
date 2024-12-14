using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Entities;

namespace Tournament.Core.Repositories
{
    public interface IGameRepository
    {
        Task<(IEnumerable<Game> Items, int TotalItems)> GetAllAsync(string title, int pageSize, int currentPage);
        Task<Game> GetAsync(int id);
        Task<bool> AnyAsync(int id);
        void Add(Game game);
        void Update(Game game);
        void Remove(Game game);
    }
}
