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
        private readonly IServiceManager serviceManager;
        private readonly IUoW _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GamesController> _logger;

        public GamesController(IUoW unitOfWork, IMapper mapper, ILogger<GamesController> logger, IServiceManager serviceManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            this.serviceManager = serviceManager;
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

            var gameDtos = _mapper.Map<IEnumerable<GameDto>>(filteredGames);

            return Ok(gameDtos);
        }


        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGameById(int id)
        {
            var game = await _unitOfWork.GameRepository.GetAsync(id);

            if (game == null) return NotFound();

            var gameDto = _mapper.Map<GameDto>(game);

            return Ok(gameDto);
        }

        // PUT: api/Games/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, GameDto gameDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingGame = await _unitOfWork.GameRepository.GetAsync(id);
            if (existingGame == null)
            {
                return NotFound();
            }

            try
            {
                _mapper.Map(gameDto, existingGame);
                _unitOfWork.GameRepository.Update(existingGame);
                await _unitOfWork.CompleteAsync();
                return Ok(_mapper.Map<GameDto>(existingGame));
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
            catch (Exception)
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
                _logger.LogInformation("Starting to map GameDto to Game");
                var game = _mapper.Map<Game>(gameDto);

                _logger.LogInformation("Adding Game to repository");
                _unitOfWork.GameRepository.Add(game);

                _logger.LogInformation("Saving changes to the database");
                await _unitOfWork.CompleteAsync();

                var createdGame = _mapper.Map<GameDto>(game);
                _logger.LogInformation($"Game created successfully with ID: {game.Id}");

                return CreatedAtAction(nameof(GetGameById), new { id = game.Id }, createdGame);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while saving the game.");
            }

            //try
            //{
            //    var game = _mapper.Map<Game>(gameDto);
            //    _unitOfWork.GameRepository.Add(game);
            //    await _unitOfWork.CompleteAsync();

            //    var createdGame = _mapper.Map<GameDto>(game);

            //    return CreatedAtAction(nameof(GetGameById), new { id = createdGame.Id }, createdGame);
            //}
            //catch (Exception)
            //{
            //    return StatusCode(500, "An error occurred while saving the game.");
            //}
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
