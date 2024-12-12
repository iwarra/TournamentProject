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

        public async Task<IEnumerable<GameDto>> GetGamesAsync()
        {
            return _mapper.Map<IEnumerable<GameDto>>(await _uow.GameRepository.GetAllAsync());
        }


        /// Not finished

        public void CreateGame(GameDto gameDto)
        {
            throw new NotImplementedException();
        }

        public void DeleteGame(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateGame(int id, GameDto gameDto)
        {
            throw new NotImplementedException();
        }
    }
}
