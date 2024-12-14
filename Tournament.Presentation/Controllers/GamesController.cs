using Microsoft.AspNetCore.Mvc;
using Tournament.Core.Entities;
using AutoMapper;
using Tournament.Core.Repositories;
using Tournament.Core.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;


namespace Tournament.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public GamesController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }


        // GET: api/Games?title={title}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGame(string title, int pageSize = 20, int currentPage = 1)
        {
            if (string.IsNullOrEmpty(title)) return BadRequest("Title must be provided.");

            pageSize = Math.Min(pageSize, 100);
            currentPage = Math.Max(currentPage, 1);

            var (filteredGames, totalItems) = await _serviceManager.GameService.GetGamesAsync(title, pageSize, currentPage);

            if (!filteredGames.Any()) return NotFound("No games found with the specified title.");

            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var response = new
            {
                Data = filteredGames,
                Metadata = new
                {
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    CurrentPage = currentPage,
                    TotalItems = totalItems
                }
            };

            return Ok(response);
        }


        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGameById(int id)
        {
            var game = await _serviceManager.GameService.GetGameById(id);

            if (game == null) return NotFound();

            return Ok(game);
        }

        // PUT: api/Games/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, GameDto gameDto)
        {
            if (!ModelState.IsValid)  return BadRequest(ModelState);

            try
            {
                var existingGame = await _serviceManager.GameService.GetGameById(id);

                if (existingGame == null) return NotFound();

                var updatedGame = await _serviceManager.GameService.UpdateGameAsync(id, existingGame);

                return Ok(updatedGame);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "A concurrency error occurred.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating the tournament.");
            }
        }

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Game>> PostGame(GameDto gameDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
         
            try
            {
                var (id, createdGame) = await _serviceManager.GameService.CreateGameAsync(gameDto);
                return CreatedAtAction(nameof(GetGameById), new { id }, createdGame);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while saving the game.");
            }
        }

        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            try
            {
                var deletedGame = await _serviceManager.GameService.DeleteGameAsync(id);
               
                return Ok(deletedGame);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while deleting the game.");
            }
        }

        [HttpPatch("{gameId}")]
        public async Task<ActionResult<GameDto>> PatchGame(int gameId, JsonPatchDocument<GameDto> patchDocument)
        {
            if (patchDocument == null) return BadRequest("Patch document cannot be null.");


            try
            {
                var game = await _serviceManager.GameService.GetGameById(gameId);
                if (game == null) return NotFound();

                patchDocument.ApplyTo(game, ModelState);
                TryValidateModel(game);

                if (!ModelState.IsValid) return UnprocessableEntity(ModelState);
                var updatedGame = await _serviceManager.GameService.UpdateGameAsync(gameId, game);

                return Ok(updatedGame);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while applying the patch.");
            }
        }

    }
}
