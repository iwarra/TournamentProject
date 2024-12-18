﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Dto;
using Tournament.Core.Entities;

namespace Service.Contracts
{
    public interface IGameService
    {
        Task<(IEnumerable<GameDto> Items, int TotalItems)> GetGamesAsync(string title, int pageSize, int currentPage);
        Task<GameDto> GetGameById(int id);
        Task<(int id, GameDto GameDto)> CreateGameAsync(GameDto gameDto, int? tournamentId = null);
        Task<GameDto> UpdateGameAsync(int id, GameDto gameDto);
        Task<GameDto> DeleteGameAsync(int id);
    }
}
