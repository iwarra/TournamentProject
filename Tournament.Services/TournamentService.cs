using AutoMapper;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Dto;
using Tournament.Core.Repositories;

namespace Tournament.Services
{
    public class TournamentService : ITournamentService
    {

        private readonly IUoW _uow;
        private readonly IMapper _mapper;

        public TournamentService(IUoW uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }
        
        public async Task<TournamentDto> GetTournamentById(int id)
        {
            var tournament = await _uow.TournamentRepository.GetAsync(id);

            if (tournament != null)
            {
                return _mapper.Map<TournamentDto>(tournament);
            }
            //ToDo: add return for if null
            return null;
        }

        public async Task<IEnumerable<TournamentDto>> GetTournamentsAsync(bool includeGames = false)
        {
            return includeGames ? _mapper.Map<IEnumerable<TournamentDto>>(await _uow.TournamentRepository.GetAllAsync(true)) : _mapper.Map<IEnumerable<TournamentDto>>(await _uow.TournamentRepository.GetAllAsync());
        }

        public void UpdateTournament(int id, TournamentDto tournamentDto)
        {
            throw new NotImplementedException();
        }

        public void CreateTournament(TournamentDto tournamentDto)
        {
            throw new NotImplementedException();
        }

        public void DeleteTournament(int id)
        {
            throw new NotImplementedException();
        }
    }
}
