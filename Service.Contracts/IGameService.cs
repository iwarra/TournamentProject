using System;
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
        Task<IEnumerable<GameDto>> GetGamesAsync();
        Task<GameDto> GetGameById(int id);
        void CreateGame(GameDto gameDto);
        void UpdateGame(int id, GameDto gameDto);
        void DeleteGame(int id);
    }
}
