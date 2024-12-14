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
            //ToDo: add return for if null
            return null;
        }

        public Task<IEnumerable<GameDto>> GetGamesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<GameDto>> GetGamesAsync(int pageSize)
        {
            return _mapper.Map<IEnumerable<GameDto>>(await _uow.GameRepository.GetAllAsync()).Take(pageSize);
        }


        /// Not finished
        public async Task<GameDto> UpdateGameAsync(int id, GameDto gameDto)
        {
            var existingGame = await _uow.GameRepository.GetAsync(id);
            if (existingGame == null)
            {
                throw new KeyNotFoundException($"Tournament with ID {id} was not found.");
            }

            _mapper.Map(gameDto, existingGame);
            _uow.GameRepository.Update(existingGame);
            await _uow.CompleteAsync();

            return gameDto;
        }

        public async Task<(int id, GameDto GameDto)> CreateGameAsync(GameDto gameDto)
        {
            var game = _mapper.Map<Game>(gameDto);
            _uow.GameRepository.Add(game);
            await _uow.CompleteAsync();

            return (game.Id, gameDto);
        }

        public async Task<GameDto> DeleteGameAsync(int id)
        {
            var game = await _uow.GameRepository.GetAsync(id);
            if (game == null)
            {
                throw new KeyNotFoundException($"Tournament with ID {id} was not found.");
            }
            _uow.GameRepository.Remove(game);
            await _uow.CompleteAsync();
            var gameDto = _mapper.Map<GameDto>(game);

            return gameDto;
        }
    }
}
