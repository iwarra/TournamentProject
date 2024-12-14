using AutoMapper;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Dto;
using Tournament.Core.Entities;
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

        public async Task<(IEnumerable<TournamentDto> Items, int TotalItems)> GetTournamentsAsync(bool includeGames = false, int pageSize = 20, int currentPage = 1)
        {
            //return includeGames ? _mapper.Map<IEnumerable<TournamentDto>>(await _uow.TournamentRepository.GetAllAsync(true)).Take(pageSize) : _mapper.Map<IEnumerable<TournamentDto>>(await _uow.TournamentRepository.GetAllAsync()).Take(pageSize);

            var (items, totalItems) = await _uow.TournamentRepository.GetAllAsync(includeGames, pageSize, currentPage);

            var tournamentDtos = _mapper.Map<IEnumerable<TournamentDto>>(items);

            return (tournamentDtos, totalItems);
        }

        public async Task<TournamentDto> UpdateTournamentAsync(int id, TournamentDto tournamentDto)
        {
            var existingTournament = await _uow.TournamentRepository.GetAsync(id);
            if (existingTournament == null)
            {
                throw new KeyNotFoundException($"Tournament with ID {id} was not found.");
            }

            _mapper.Map(tournamentDto, existingTournament);
            _uow.TournamentRepository.Update(existingTournament);
            await _uow.CompleteAsync();

            return tournamentDto;
        }

        public async Task<(int id,TournamentDto TournamentDto)> CreateTournamentAsync(TournamentDto tournamentDto)
        {
            var tournament = _mapper.Map<TournamentDetails>(tournamentDto);
            _uow.TournamentRepository.Add(tournament);
            await _uow.CompleteAsync();

            return (tournament.Id, tournamentDto);
        }

        public async Task<TournamentDto> DeleteTournamentAsync(int id)
        {
            var tournament = await _uow.TournamentRepository.GetAsync(id);
            if (tournament == null)
            {
                throw new KeyNotFoundException($"Tournament with ID {id} was not found.");
            }
            _uow.TournamentRepository.Remove(tournament);
            await _uow.CompleteAsync();
            var tournamentDto = _mapper.Map<TournamentDto>(tournament);

            return tournamentDto;
        }
    }
}
