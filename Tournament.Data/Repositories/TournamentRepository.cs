using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Tournament.Data.Repositories
{
    public class TournamentRepository :  ITournamentRepository
    {
        private readonly TournamentApiContext _context;

        public TournamentRepository(TournamentApiContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<TournamentDetails> Items, int TotalItems)> GetAllAsync(bool includeGames, int pageSize, int currentPage)
        {
            //return includeGames ? await _context.TournamentDetails.Include(t => t.Games).ToListAsync() : await _context.TournamentDetails.ToListAsync();
            var query = _context.TournamentDetails.AsQueryable();

            if (includeGames)
            {
                query = query.Include(t => t.Games);
            }

            var totalItems = await query.CountAsync(); 

            var items = await query
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)                   
                .ToListAsync();

            return (items, totalItems);
        }

        public async Task<TournamentDetails> GetAsync(int id)
        {
            return await _context.TournamentDetails.FindAsync(id);
        }

        public async Task<bool> AnyAsync(int id)
        {
            return await _context.TournamentDetails.AnyAsync(t => t.Id == id);
        }

        public void Add(TournamentDetails tournament)
        {
            _context.TournamentDetails.Add(tournament);
        }

        public void Update(TournamentDetails tournament)
        {
            _context.TournamentDetails.Update(tournament);
        }

        public void Remove(TournamentDetails tournament)
        {
            _context.TournamentDetails.Remove(tournament);
        }
    }
}
