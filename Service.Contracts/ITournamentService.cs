﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Dto;
using Tournament.Core.Entities;

namespace Service.Contracts
{
    public interface ITournamentService
    {
        Task<IEnumerable<TournamentDto>> GetTournamentsAsync(bool includeGames = false);
        Task<TournamentDto> GetTournamentById(int id);
        Task<(int id, TournamentDto TournamentDto)> CreateTournamentAsync(TournamentDto tournamentDto);
        Task<TournamentDto> UpdateTournamentAsync(int id, TournamentDto tournamentDto);
        Task<TournamentDto> DeleteTournamentAsync(int id); 
    }
}
