﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;

namespace Tournament.Data.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly TournamentApiContext _context;

        public GameRepository(TournamentApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Game>> GetAllAsync()
        {
            return await _context.Game.ToListAsync();
        }

        public async Task<Game> GetAsync(int id)
        {
            return await _context.Game.FindAsync(id);
        }

        public async Task<bool> AnyAsync(int id)
        {
            return await _context.Game.AnyAsync(g => g.Id == id);
        }

        public void Add(Game game)
        {
            _context.Game.Add(game);
        }

        public void Update(Game game)
        {
            _context.Game.Update(game);
        }

        public void Remove(Game game)
        {
            _context.Game.Remove(game);
        }
    }
}