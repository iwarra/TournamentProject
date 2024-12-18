﻿using AutoMapper;
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
    public class GameService : IGameService
    {
        private readonly IUoW _uow;
        private readonly IMapper _mapper;

        public GameService(IUoW uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<GameDto> GetGameById(int id)
        {
            var game = await _uow.GameRepository.GetAsync(id);

            if (game != null)
            {
                return _mapper.Map<GameDto>(game);
            }
            else throw new GameNotFoundException(id);
        }

        public async Task<(IEnumerable<GameDto> Items, int TotalItems)> GetGamesAsync(string title, int pageSize, int currentPage)
        {
            var (items, totalItems) = await _uow.GameRepository.GetAllAsync(title, pageSize, currentPage);

            var gameDtos = _mapper.Map<IEnumerable<GameDto>>(items);

            return (gameDtos, totalItems);
        }


        /// Not finished
        public async Task<GameDto> UpdateGameAsync(int id, GameDto gameDto)
        {
            var existingGame = await _uow.GameRepository.GetAsync(id);
            if (existingGame == null)
            {
                throw new GameNotFoundException(id);
            }

            _mapper.Map(gameDto, existingGame);
            _uow.GameRepository.Update(existingGame);
            await _uow.CompleteAsync();

            return gameDto;
        }

        public async Task<(int id, GameDto GameDto)> CreateGameAsync(GameDto gameDto, int? tournamentId)
        {

            try
            {
                TournamentDetails tournament = null;

               if (tournamentId.HasValue)
                {
                    tournament = await _uow.TournamentRepository.GetAsync(tournamentId.Value);
                }
                if (tournament == null)
                {
                    throw new TournamentNotFoundException(tournamentId.Value);
                }
                if (tournament.Games.Count >= 10)
                {
                    throw new GameLimitExceededException(tournament.Title);
                }

                var game = _mapper.Map<Game>(gameDto);
                if (tournament != null)
                {
                    tournament.Games.Add(game); 
                }
                _uow.GameRepository.Add(game);
                await _uow.CompleteAsync();

                return (game.Id, gameDto);
            }

            catch (TournamentNotFoundException ex)
            {
                throw;
            }
            catch (GameLimitExceededException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<GameDto> DeleteGameAsync(int id)
        {
            var game = await _uow.GameRepository.GetAsync(id);
            if (game == null)
            {
                throw new GameNotFoundException(id);
            }
            _uow.GameRepository.Remove(game);
            await _uow.CompleteAsync();
            var gameDto = _mapper.Map<GameDto>(game);

            return gameDto;
        }
    }
}
