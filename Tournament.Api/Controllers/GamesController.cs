using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tournament.Data.Data;
using Tournament.Core.Entities;
using AutoMapper;
using Tournament.Core.Repositories;
using Tournament.Core.Dto;
using Microsoft.AspNetCore.JsonPatch;

namespace Tournament.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IUoW _unitOfWork;
        private readonly IMapper _mapper;

        public GamesController(IUoW unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //GET: api/Games
       //[HttpGet]
       // public async Task<ActionResult<IEnumerable<Game>>> GetGame()
       // {
       //     var games = await _unitOfWork.GameRepository.GetAllAsync();

       //     var gameDtos = _mapper.Map<IEnumerable<GameDto>>(games);

       //     return Ok(gameDtos);
       // }

        // GET: api/Games?title={title}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGame(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return BadRequest("Title must be provided.");
            }

            var games = await _unitOfWork.GameRepository.GetAllAsync();

            var filteredGames = games.Where(g => g.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

            if (!filteredGames.Any())
            {
                return NotFound("No games found with the specified title.");
            }

            // Map filtered games to DTOs
            var gameDtos = _mapper.Map<IEnumerable<GameDto>>(filteredGames);

            return Ok(gameDtos);
        }


        // GET: api/Games/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Game>> GetGameById(int id)
        //{
        //    var game = await _unitOfWork.GameRepository.GetAsync(id);

        //    if (game == null) return NotFound();

        //    var gameDto = _mapper.Map<GameDto>(game);

        //    return Ok(gameDto);
        //}

        // PUT: api/Games/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, GameDto gameDto)
        {
            if (id != gameDto.Id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var game = _mapper.Map<Game>(gameDto);
                _unitOfWork.GameRepository.Update(game);
                await _unitOfWork.CompleteAsync();
                return Ok(gameDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                bool isFound = await GameExists(id);
                if (!isFound)
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode(500, "A concurrency error occurred.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the game.");
            }

        }

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Game>> PostGame(GameDto gameDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var game = _mapper.Map<Game>(gameDto);
                _unitOfWork.GameRepository.Add(game);
                await _unitOfWork.CompleteAsync();

                var createdGameDto = _mapper.Map<GameDto>(game);

                return CreatedAtAction(nameof(GetGame), new { id = createdGameDto.Id }, createdGameDto);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, "An error occurred while saving the game.");
            }
        }

        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _unitOfWork.GameRepository.GetAsync(id);

            if (game == null) return NotFound();

            try
            {
                _unitOfWork.GameRepository.Remove(game);
                await _unitOfWork.CompleteAsync();

                var deletedGameDto = _mapper.Map<GameDto>(game);

                return Ok(deletedGameDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the game.");
            }
        }

        [HttpPatch("{gameId}")]
        public async Task<ActionResult<GameDto>> PatchGame(int gameId, JsonPatchDocument<GameDto> patchDocument)
        {
            if (patchDocument == null) return BadRequest("Patch document cannot be null.");

            var game = await _unitOfWork.GameRepository.GetAsync(gameId);
            if (game == null) return NotFound();

            var gameDto = _mapper.Map<GameDto>(game);

            // Apply the patch
            patchDocument.ApplyTo(gameDto, ModelState);
            TryValidateModel(gameDto);

            if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

            try
            {
                var updatedGame = _mapper.Map(gameDto, game);

                _unitOfWork.GameRepository.Update(updatedGame);
                await _unitOfWork.CompleteAsync();

                return Ok(gameDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while applying the patch.");
            }
        }



        private async Task<bool> GameExists(int id)
        {
            return await _unitOfWork.GameRepository.AnyAsync(id);
        }
    }
}
