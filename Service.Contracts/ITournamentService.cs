using System;
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
        void CreateTournament(TournamentDto tournamentDto);
        void UpdateTournament(int id, TournamentDto tournamentDto);
        void DeleteTournament(int id);
        
    }
}
